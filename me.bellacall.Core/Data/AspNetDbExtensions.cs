using me.bellacall.Core.Locales;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data
{
    public static class AspNetDbExtensions
    {
        /// <summary>
        /// Возвращает название таблицы из Repository
        /// </summary>
        /// <typeparam name="TEntity">Тип сущности</typeparam>
        /// <param name="table">Таблица (property из Repository)</param>
        public static string GetName<TEntity>(this DbSet<TEntity> table) where TEntity : class
        {
            return typeof(AspNetDbContext)
                .GetProperties()
                .Where(p => p.PropertyType.GenericTypeArguments.Length == 1 && p.PropertyType.GenericTypeArguments[0] == typeof(TEntity))
                .Last()
                .Name;
        }

        /// <summary>
        /// Возвращает таблицу из Repository по заданному типу сущности
        /// </summary>
        /// <typeparam name="TEntity">Тип сущности</typeparam>
        /// <param name="db">Экземпляр Repository</param>
        public static DbSet<TEntity> GetTable<TEntity>(this AspNetDbContext db) where TEntity : class, IEntity
        {
            return (DbSet<TEntity>)typeof(AspNetDbContext)
                .GetProperties()
                .Where(p => p.PropertyType.GenericTypeArguments.Length == 1 && p.PropertyType.GenericTypeArguments[0] == typeof(TEntity))
                .Last()
                .GetValue(db);
        }

        /// <summary>
        /// Вычисляет hash по свойствам с атрибутом HashPart
        /// </summary>
        /// <param name="entity"></param>
        public static string GetHash(this IEntity entity)
        {
            var json = "{" + string.Join(",", entity.GetType().GetProperties()
               .Where(p => p.CustomAttributes
               .Any(a => a.AttributeType == typeof(HashPartAttribute)))
               .Select(p => $"\"{p.Name}\":\"{Convert.ToString(p.GetValue(entity))}\"")) + "}";

            var bytes = Encoding.UTF8.GetBytes(json);

            using (var hasher = MD5.Create())
            {
                var hash = hasher.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "");
            }
        }

        /// <summary>
        /// Возвращает описания всех таблиц
        /// </summary>
        public static IEnumerable<TableFeature> GetTableFeatures()
        {
            return typeof(AspNetDbContext)
                .GetProperties()
                .Where(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(HasPermissionsAttribute)))
                .Select(p => GetTableFeature(p));
        }

        /// <summary>
        /// Возвращает описание таблицы
        /// </summary>
        /// <param name="name">Название таблицы</param>
        public static TableFeature GetTableFeature(string name)
        {
            return GetTableFeature(typeof(AspNetDbContext)
                .GetProperties()
                .SingleOrDefault(p => p.Name == name && p.CustomAttributes.Any(a => a.AttributeType == typeof(HasPermissionsAttribute))));
        }

        private static TableFeature GetTableFeature(PropertyInfo property)
        { return new TableFeature(property.Name, property.CustomAttributes.Any(a => a.AttributeType == typeof(DisplayAttribute)) ? property.GetCustomAttribute<DisplayAttribute>().ResourceType != typeof(Strings) ? property.GetCustomAttribute<DisplayAttribute>().Name : Strings.ResourceManager.GetString(property.GetCustomAttribute<DisplayAttribute>().Name) : property.Name, property.GetCustomAttribute<HasPermissionsAttribute>().Operations); }
    }

    /// <summary>
    /// Включает поле в расчет хэша
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class HashPartAttribute : Attribute { }

    /// <summary>
    /// Описание таблицы
    /// </summary>
    public class TableFeature
    {
        public TableFeature(string name, string displayName, IEnumerable<Operation> operations)
        {
            Name = name;
            DisplayName = displayName;
            Operations = operations;
        }

        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Отображаемое название
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Доступные операции
        /// </summary>
        public IEnumerable<Operation> Operations { get; private set; }
    }
}
