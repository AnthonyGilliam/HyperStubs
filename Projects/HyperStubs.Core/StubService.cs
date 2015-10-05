﻿using System.Collections.Generic;
using System.Linq;
using HyperStubs.Core;
using HyperStubs.Enums;

namespace HyperStubs
{
    /// <summary>
    ///     Provides functionallity for generating a stubbed object.
    /// </summary>
    public class StubService : IStubService
    {
        /// <summary>
        ///     Generically instantiates *any* Domain Model Object with pre-populated property data
        /// </summary>
        /// <typeparam name="T">Type of Business Object to be stubbed</typeparam>
        /// <returns>Fully instantiated and stubbed Business Object</returns>
        public T GetStubbedObject<T>()
        {
            return DataGenerator.GenerateRandomObject<T>();
        }

        /// <summary>
        ///     This generically instantiates *any* Domain Model Object with pre-populated property data
        /// </summary>
        /// <typeparam name="T">Type of Business Object to be stubbed</typeparam>
        /// <param name="propertyDataTypes">
        ///     List of data types to be assigned to stubbed Business Object properties in respective order
        ///     of the properties defined in the given Business Object Type.  If the property types don't match,
        ///     random values will be assigned to the properties instead.
        /// </param>
        /// <returns>Fully instantiated and stubbed Business Object</returns>
        public T GetStubbedObject<T>(params StubDataType[] propertyDataTypes) where T : new()
        {
            var propertyTypeList = new List<StubDataType>();
            
            propertyTypeList.AddRange(propertyDataTypes);

            return DataGenerator.GenerateRandomObject<T>(propertyTypeList);
        }

        /// <summary>
        ///     This generically instantiates *any* Domain Model Object with pre-populated property data
        /// </summary>
        /// <typeparam name="T">Type of Business Object to be stubbed</typeparam>
        /// <param name="propertyNames">Names of porperties to be given a specific stub type</param>
        /// <param name="propertyDataTypes">
        ///     Data types to be assigned to given property names in respective order of the
        ///     propertyNames array
        /// </param>
        /// <returns>Fully instantiated and stubbed Business Object</returns>
        public T GetStubbedObject<T>(string[] propertyNames, params StubDataType[] propertyDataTypes)
            where T : new()
        {
            return DataGenerator.GenerateRandomObject<T>(propertyNames, propertyDataTypes);
        }

        /// <summary>
        ///     This generically instantiates *any* Domain Model Object with pre-populated property data
        /// </summary>
        /// <typeparam name="T">Type of Business Object to be stubbed</typeparam>
        /// <param name="propertyTypeIndexDictionary">Key-value pairs containing the index of a Business Object's property and the data type to stub it with</param>
        public T GetStubbedObject<T>(IDictionary<int, StubDataType> propertyTypeIndexDictionary) where T : new()
        {
            var propertyTypeList = new List<StubDataType>(typeof(T).GetProperties().Count());

            foreach (var pair in propertyTypeIndexDictionary)
            {
                propertyTypeList[pair.Key] = pair.Value;
            }

            return DataGenerator.GenerateRandomObject<T>(propertyTypeList);
        }
    }
}
