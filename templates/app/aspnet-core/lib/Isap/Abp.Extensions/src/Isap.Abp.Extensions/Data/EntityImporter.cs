using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Isap.CommonCore.Accessors;
using Isap.CommonCore.Collections;
using Isap.CommonCore.Services;

namespace Isap.Abp.Extensions.Data
{
	public interface IEntityImporter<TIntf, TImpl>
		where TIntf: ICommonEntity<Guid>
		where TImpl: class, TIntf, IAssignable<Guid, TIntf>, new()
	{
		Task<List<TImpl>> ImportAsync(DirectoryInfo baseDir);
		Task ImportAsync(DirectoryInfo baseDir, Func<TImpl, Task> import);
	}

	public abstract class EntityImporter<TIntf, TImpl>: IEntityImporter<TIntf, TImpl>
		where TIntf: ICommonEntity<Guid>
		where TImpl: class, TIntf, IAssignable<Guid, TIntf>, new()
	{
		private readonly IEntityAccessor<TImpl> _emailTemplateAccessor = EntityAccessors.Get<TImpl>();

		private readonly string _rootElementName;

		protected EntityImporter(string rootElementName)
		{
			_rootElementName = rootElementName;
		}

		public virtual async Task<List<TImpl>> ImportAsync(DirectoryInfo baseDir)
		{
			var result = new List<TImpl>();
			await ImportAsync(baseDir, entry =>
				{
					result.Add(entry);
					return Task.CompletedTask;
				});
			return result;
		}

		public virtual async Task ImportAsync(DirectoryInfo baseDir, Func<TImpl, Task> import)
		{
			if (baseDir.Exists)
			{
				foreach (DirectoryInfo directory in baseDir.GetDirectories())
					await ImportAsync(directory);
				foreach (FileInfo file in baseDir.GetFiles("*.xml"))
				{
					XElement rootElement = XElement.Load(file.FullName);
					if (rootElement.Name != _rootElementName)
						throw new InvalidOperationException($"Unexpected root element name '{rootElement.Name}' in file {file.FullName}.");
					var template = new TImpl();
					foreach (XAttribute attribute in rootElement.Attributes())
						_emailTemplateAccessor.SetValue(template, attribute.Name.LocalName, attribute.Value);
					foreach (XElement element in rootElement.Elements(typeof(TImpl).Name))
					{
						var item = new TImpl();
						item.Assign(template);
						ImportItem(item, element, _emailTemplateAccessor);
						await import(item);
					}
				}
			}
		}

		private void ImportItem(object item, XElement element, IEntityAccessor entityAccessor)
		{
			foreach (XAttribute attribute in element.Attributes())
				entityAccessor.SetValue(item, attribute.Name.LocalName, attribute.Value);
			foreach (XElement childElement in element.Elements())
			{
				IPropertyAccessor propertyAccessor = entityAccessor.GetProperty(childElement.Name.LocalName);
				if (childElement.HasElements)
				{
					Type propType = propertyAccessor.GetPropertyType();
					object propValue = GetCollectionValue(propType, childElement.Elements());
					propertyAccessor.SetValue(item, propValue);
				}
				else
					propertyAccessor.SetValue(item, childElement.Value);
			}
		}

		private object GetCollectionValue(Type collectionType, IEnumerable<XElement> elements)
		{
			if (!typeof(ICollection).IsAssignableFrom(collectionType))
				throw new InvalidOperationException();

			if (collectionType.IsGenericType)
			{
				Type[] genericArguments = collectionType.GetGenericArguments();
				if (genericArguments.Length != 1)
					throw new InvalidOperationException();

				Type itemType = genericArguments[0];
				ICollectionBuilder builder = CreateCollectionBuilder(itemType, elements);

				Type genericTypeDef = collectionType.GetGenericTypeDefinition();

				if (genericTypeDef == typeof(List<>))
				{
					return builder.ToGenericList();
				}

				if (genericTypeDef == typeof(IList<>))
				{
					return builder.ToGenericList();
				}

				if (genericTypeDef == typeof(ICollection<>))
				{
					return builder.ToGenericCollection();
				}
			}
			else if (collectionType.IsArray)
			{
				Type itemType = collectionType.GetElementType();
				if (itemType == null)
					throw new InvalidOperationException();

				ICollectionBuilder builder = CreateCollectionBuilder(itemType, elements);

				return builder.ToArray();
			}

			throw new InvalidOperationException();
		}

		private ICollectionBuilder CreateCollectionBuilder(Type itemType, IEnumerable<XElement> elements)
		{
			ICollectionBuilder builder = CollectionBuilder.Create(itemType);

			IEntityAccessor itemAccessor = EntityAccessors.Get(itemType);
			object item = Activator.CreateInstance(itemType);
			foreach (XElement element in elements)
			{
				ImportItem(item, element, itemAccessor);
				builder.Add(item);
			}

			return builder;
		}
	}
}
