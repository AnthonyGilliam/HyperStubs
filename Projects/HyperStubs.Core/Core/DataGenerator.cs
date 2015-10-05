using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using HyperStubs.ExtensionMethods;
using HyperStubs.Enums;
using HyperStubs.Random;

namespace HyperStubs.Core
{
    internal static class DataGenerator
    {
        private const int MAX_PROPERTY_DEPTH_COUNT = 5;
        private const int MAX_RANDOM_COLLECTION_COUNT = 5;

        private static int _iteration;

        private static readonly Func<Type, bool> IsPrimitiveType = objType => objType.IsPrimitive
                                                                              || objType == typeof (decimal)
                                                                              || objType == typeof (string)
                                                                              || objType == typeof (Guid)
                                                                              || objType == typeof (DateTime);

        private static readonly Func<Type, bool> IsNullableType =
            objType => objType.IsGenericType && objType.GetGenericTypeDefinition() == typeof (Nullable<>);

        private static readonly Func<Type, bool> IsCollectionType =
            objType => (objType.IsGenericType || objType.IsArray) && !(IsNullableType(objType));

        /// <summary>
        /// A stack to hold the last known parent object while recursively stubbing an object's properties.
        /// Its count may be as much as the MAX_PROPERTY_DEPTH_COUNT value.
        /// </summary>
        private static readonly Stack<object> _parentObjectStack;
        /// <summary>
        /// A dictionary to hold iterative one-to-many parent objects during recursive property stubbing.
        /// </summary>
        private static readonly IDictionary<Type, object> _parentOneToManyObjectHash;
        /// <summary>
        /// A dictionary to hold iterative one-to-one parent objects during recursive property stubbing.
        /// </summary>
        private static readonly IDictionary<Type, object> _parentOneToOneObjectHash;

        static DataGenerator()
        {
            _parentOneToOneObjectHash = new Dictionary<Type, Object>();
            _parentOneToManyObjectHash = new Dictionary<Type, object>();
            _parentObjectStack = new Stack<object>();
        }

        /// <summary>
        ///     This generically instantiates *any* Domain Model Object with pre-populated property data
        /// </summary>
        /// <typeparam name="T">Type of Business Object to be stubbed</typeparam>
        /// <returns>Fully instantiated and stubbed Business Object</returns>
        internal static T GenerateRandomObject<T>()
        {
            _iteration = 0;

            var generatedObject = InterrogateType(typeof (T), null, 0);

            return (T) generatedObject;
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
        internal static T GenerateRandomObject<T>(IList<string> propertyNames, IList<StubDataType> propertyDataTypes)
            where T : new()
        {
            var objType = typeof (T);
            var propertyInfoArray = objType.GetProperties();
            var stubObject = InvokeNewObject(objType);

            foreach (var propertyInfo in propertyInfoArray)
            {
                var propType = propertyInfo.PropertyType;
                object propObj;

                if (propertyNames.Any(n => n == propertyInfo.Name))
                {
                    var dataTypeIndex = propertyNames.IndexOf(propertyInfo.Name);
                    var stubType = propertyDataTypes[dataTypeIndex];
                    propObj = DataTypeEvaluator.GenerateStubObject(propType, stubType);
                }
                else
                {
                    propObj = InterrogateType(propType, propertyInfo.Name, _iteration);
                }

                propertyInfo.SetValue(stubObject
                    , propObj
                    , null);
            }

            return (T) stubObject;
        }

        /// <summary>
        ///     This generically instantiates *any* Domain Model Object with pre-populated property data
        /// </summary>
        /// <typeparam name="T">Type of Business Object to be stubbed</typeparam>
        /// <param name="propertyTypeList">
        ///     List of data types to be assigned to stubbed Business Object properties in respective order
        ///     of the properties defined in the given Business Object Type
        /// </param>
        /// <returns>Fully instantiated and stubbed Business Object</returns>
        internal static T GenerateRandomObject<T>(IList<StubDataType> propertyTypeList) where T : new()
        {
            var objType = typeof (T);
            var propertyInfoArray = objType.GetProperties();
            var stubObject = InvokeNewObject(objType);

            if (propertyInfoArray.Count() != propertyTypeList.Count())
            {
                return (T) stubObject;
            }

            var i = 0;
            foreach (var stubDataType in propertyTypeList)
            {
                var propertyInfo = propertyInfoArray[i];
                var propType = propertyInfo.PropertyType;

                var propObj = stubDataType != StubDataType.None
                    ? DataTypeEvaluator.GenerateStubObject(propType, stubDataType)
                    : InterrogateType(propType, propertyInfo.Name, _iteration);

                propertyInfo.SetValue(stubObject
                    , propObj
                    , null);

                i++;
            }

            return (T) stubObject;
        }

        /// <summary>
        ///     Analyzes object type to be stubbed and determines what stub data to assign to it
        /// </summary>
        /// <param name="objType">Object to be stubbed</param>
        /// <param name="propertyName">Stub object property name (used if stub object is a property of another object)</param>
        /// <param name="iteration">Property debth count. Used to limit the recursion of calls to this method</param>
        /// <returns>Fully instantiated and stubbed Business Object</returns>
        private static object InterrogateType(Type objType, string propertyName, int iteration)
        {
            _iteration = iteration;
            _iteration++;

            object stubObject = null;

            try
            {
                if (IsPrimitiveType(objType))
                {
                    stubObject = EvaluateType(objType, propertyName);
                }
                else if (objType.IsEnum)
                {
                    stubObject = StubEnum(objType);
                }
                else if (IsNullableType(objType))
                {
                    stubObject = InvokeNewObject(objType);
                }
                else if (IsCollectionType(objType))
                {
                    if (_parentObjectStack.Count > 0
                        && _parentObjectStack.Peek() != null
                        && !_parentOneToManyObjectHash.ContainsKey(_parentObjectStack.Peek().GetType()))
                    {
                        _parentOneToManyObjectHash.Add(_parentObjectStack.Peek().GetType(), _parentObjectStack.Peek());
                    }

                    stubObject = StubCollection(objType);

                    if (_parentObjectStack.Count > 0)
                        _parentOneToManyObjectHash.Remove(_parentObjectStack.Peek().GetType());
                }
                else if (_iteration <= MAX_PROPERTY_DEPTH_COUNT)
                {
                    try
                    {
                        stubObject = InvokeNewObject(objType);
                        if (stubObject == null)
                        {
                            _iteration--;
                            return null;
                        }
                        _parentObjectStack.Push(stubObject);
                        _parentOneToOneObjectHash.Add(objType, stubObject);

                        var propertyInfoArray = objType.GetProperties();
                        var sortedPropertyInfoArray = SortObjectProperties(propertyInfoArray);
                        foreach (var propertyInfo in sortedPropertyInfoArray)
                        {
                            try
                            {
                                var propType = propertyInfo.PropertyType;
                                object propObj;

                                //A seperate hash exists for one-to-many objects because their types are identified in another if statement earlier in this method; 
                                //thus it is not loaded into a hash in this scope.
                                if (_parentOneToManyObjectHash.ContainsKey(propType))
                                {
                                    propObj = _parentOneToManyObjectHash[propType];
                                }
                                //Each time a complex object is found and it is not a Collection Type, the _parentOneToOneObjectHash should be queried to 
                                //see if the propType has been hashed already.  If so, than the hashed object should be used as the propObj value
                                else if (_parentOneToOneObjectHash.ContainsKey(propType))
                                {
                                    propObj = _parentOneToOneObjectHash[propType];
                                }
                                else
                                {
                                    propObj = InterrogateType(propType, propertyInfo.Name, _iteration);
                                }

                                //Now that the property object has been evaluated, it's time to assign it to its parent object.
                                if (!IsPrimitiveType(propType) && propObj is IEnumerable)
                                {
                                    SetCollectionValue(ref stubObject
                                        , propObj
                                        , propertyInfo);
                                }
                                else
                                {
                                    //TODO: Figure out how to set private properties in base object class.
                                    propertyInfo.SetValue(stubObject
                                        , propObj
                                        , null);
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                    catch
                    {
                        if (_parentOneToOneObjectHash.ContainsKey(objType))
                        {
                            _parentOneToOneObjectHash.Remove(objType);
                        }
                    }
                    finally
                    {
                        if (stubObject != null)
                        {
                            _parentObjectStack.Pop();
                            _parentObjectStack.TrimExcess();

                            _parentOneToOneObjectHash.Remove(objType);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            _iteration--;

            return stubObject;
        }

        /// <summary>
        ///     Sorts an objects properties in order of primitive types, nullable types, complex types then collection types.
        /// </summary>
        /// <param name="propertyInfoArray"></param>
        /// <returns></returns>
        private static IEnumerable<PropertyInfo> SortObjectProperties(PropertyInfo[] propertyInfoArray)
        {
            var primitiveTypes = propertyInfoArray.Where(p => IsPrimitiveType(p.PropertyType)).ToList();
            var nullableTypes = propertyInfoArray.Where(p => IsNullableType(p.PropertyType)).ToList();
            var collectionTypes =
                propertyInfoArray.Where(p => IsCollectionType(p.PropertyType)).ToList();
            var complexTypes = propertyInfoArray.Where(p => !IsPrimitiveType(p.PropertyType)
                                                                            && !IsNullableType(p.PropertyType)
                                                                            && !IsCollectionType(p.PropertyType))
                .ToList();

            var sortedProperties =
                primitiveTypes.Union(nullableTypes).Union(complexTypes).Union(collectionTypes).Distinct().ToArray();

            return sortedProperties;
        }

        /// <summary>
        ///     Used to assign stub data to primitive data types.
        /// </summary>
        /// <param name="objType">Type of object to be stubbed</param>
        /// <param name="property_field_Name">Object name (used to determine the StubType of the resulting object)</param>
        /// <returns></returns>
        private static object EvaluateType(Type objType, string property_field_Name)
        {
            object stubObject = null;

            if (property_field_Name == null)
            {
                return EvaluateType(objType);
            }

            try
            {
                //TODO: Add types to DataTypeEvaluator
                switch (objType.Name)
                {
                    case "Boolean":
                        stubObject = Randomizer.RandomBool();
                        break;
                    case "Byte":
                        stubObject = DataTypeEvaluator.EvaluateByte(property_field_Name);
                        break;
                    case "Char":
                        stubObject = Randomizer.RandomChar(true);
                        break;
                    case "DateTime":
                        stubObject = Randomizer.RandomDate();
                        break;
                    case "Decimal":
                        stubObject = DataTypeEvaluator.EvaluateDecimal(property_field_Name);
                        break;
                    case "Double":
                        stubObject = Randomizer.RandomDouble(-9999, 9999);
                        break;
                    case "Guid":
                        stubObject = Guid.NewGuid();
                        break;
                    case "Int16":
                    case "Int32":
                    case "Int64":
                        stubObject = DataTypeEvaluator.EvaluateInt(property_field_Name);
                        break;
                    case "IntPtr":
                        stubObject = Randomizer.RandomIntPtr();
                        break;
                    case "SByte":
                        stubObject = Randomizer.RandomSignedByte();
                        break;
                    case "String":
                        stubObject = DataTypeEvaluator.EvaluateString(property_field_Name);
                        break;
                }
            }
            catch(Exception ex)
            {
            }

            return stubObject;
        }

        /// <summary>
        ///     Used to assign stub data to primitive data types.
        /// </summary>
        /// <param name="objType">Type of object to be stubbed</param>
        /// <returns></returns>
        private static object EvaluateType(Type objType)
        {
            object stubObject = null;

            try
            {
                switch (objType.Name)
                {
                    case "Boolean":
                        stubObject = Randomizer.RandomBool();
                        break;
                    case "Byte":
                        stubObject = Randomizer.RandomByte();
                        break;
                    case "Char":
                        stubObject = Randomizer.RandomChar(true);
                        break;
                    case "DateTime":
                        stubObject = Randomizer.RandomDate();
                        break;
                    case "Decimal":
                        stubObject = Randomizer.RandomDecimal();
                        break;
                    case "Double":
                        stubObject = Randomizer.RandomDouble(-9999, 9999);
                        break;
                    case "Guid":
                        stubObject = Guid.NewGuid();
                        break;
                    case "Int16":
                        stubObject = Randomizer.RandomShort(1, 999);
                        break;
                    case "Int32":
                        stubObject = Randomizer.RandomInt(1, 9999);
                        break;
                    case "Int64":
                        stubObject = Randomizer.RandomLong();
                        break;
                    case "IntPtr":
                        stubObject = Randomizer.RandomIntPtr();
                        break;
                    case "SByte":
                        stubObject = Randomizer.RandomSignedByte();
                        break;
                    case "String":
                        stubObject = Randomizer.RandomWord();
                        break;
                    case "UInt16":
                        stubObject = (UInt16)Randomizer.RandomShort(short.MaxValue);
                        break;
                    case "UInt32":
                        stubObject = (UInt32)Randomizer.RandomInt(int.MaxValue);
                        break;
                    case "UInt64":
                        stubObject = (UInt64)Randomizer.RandomLong(long.MaxValue);
                        break;
                    case "UIntPtr":
                        stubObject = new UIntPtr((uint)Randomizer.RandomInt(int.MaxValue));
                        break;
                }
            }
            catch
            {
            }

            return stubObject;
        }

        /// <summary>
        ///     Dynamically chooses a constructor and instantiates an object of given type from scratch
        /// </summary>
        /// <param name="objType">The Type of Object to be instantiated</param>
        /// <returns></returns>
        private static object InvokeNewObject(Type objType)
        {
            var objCtorInfo = objType.GetConstructors();
            ConstructorInfo sglCtorInfo;
            object newObject;

            if (objCtorInfo.Any(ctor => !ctor.GetParameters().Any()))
            {
                sglCtorInfo = objType.GetConstructor(new Type[] {});
                newObject = sglCtorInfo.Invoke(new object[] {});
            }
            else
            {
                sglCtorInfo = objCtorInfo.FirstOrDefault(ctor => ctor.GetParameters().All(p => IsPrimitiveType(p.ParameterType)));
                if (sglCtorInfo == null)
                {
                    //TODO: Stub complex objects in constructor of object to be invoked.
                    return null;
                }

                var paramInfoArray = sglCtorInfo.GetParameters();

                //TODO: Pass Property Name into Constructor for Random Object
                newObject = sglCtorInfo.Invoke(
                    paramInfoArray.Select(paramInfo => InterrogateType(paramInfo.ParameterType, null, _iteration))
                    .ToArray());
            }

            return newObject;
        }

        private static object StubEnum(Type objType)
        {
            var maxIndex = Enum.GetValues(objType).Length;
            return Enum.ToObject(objType, Randomizer.RandomInt(0, maxIndex));
        }

        /// <summary>
        ///     Used to stub collection types (types that inherit from IEnumerable<>)
        /// </summary>
        /// <param name="collectionType">Collection type to be stubbed</param>
        /// <returns></returns>
        private static object StubCollection(Type collectionType)
        {
            var genericArgs = collectionType.GetGenericArguments();
            var genericTypeName = collectionType.GetGenericTypeDefinition().Name;
            object collectionObject = null;
            
            if (genericArgs.Count() == 1)
            {
                if (collectionType.IsInterface)
                {
                    switch (genericTypeName)
                    {
                        case "IEnumerable`1":
                        case "ICollection`1":
                            var collection = typeof (Collection<>);
                            collectionType = collection.MakeGenericType(genericArgs);
                            break;
                        case "IList`1":
                            var list = typeof (List<>);
                            collectionType = list.MakeGenericType(genericArgs);
                            break;
                        //TODO:  Handle other collection types.
                    }
                }

                collectionObject = Activator.CreateInstance(collectionType);
                StubCollection(collectionObject, genericArgs[0]);
            }
            else if (genericArgs.Count() > 1)
            {
                collectionObject = StubMultiGenericCollection(collectionType, genericTypeName, genericArgs);
            }

            return collectionObject;
        }

        /// <summary>
        ///     Used to stub collection types (types that inherit from IEnumerable<>)
        /// </summary>
        /// <param name="collectionObject">Collection type to be stubbed</param>
        /// <param name="genericParamType">Pre-identified generic parameter</param>
        /// <returns></returns>
        private static object StubCollection(object collectionObject, Type genericParamType)
        {
            var collectionCount = Randomizer.RandomInt(1, MAX_RANDOM_COLLECTION_COUNT);
            for (var i = 0; i < collectionCount; i++)
            {
                var childObject = InterrogateType(genericParamType, null, _iteration);
                AddToCollection(ref collectionObject, childObject);
            }

            return collectionObject;
        }

        /// <summary>
        ///     Used to stub multi-dimensional collection types (i.e. IDictionary<>)
        /// </summary>
        /// <param name="collectionType">Collection type to be stubbed</param>
        /// <param name="genericParamType">Pre-identified generic parameter</param>
        /// <returns></returns>
        private static object StubMultiGenericCollection(Type collectionType, string typeName, Type[] genericParamTypes)
        {
            Type baseType;
            object collectionObject = null;

            if (genericParamTypes.Count() == 2)
            {
                switch (typeName)
                {
                    case "IDictionary`2":
                        baseType = typeof (Dictionary<,>);
                        collectionType = baseType.MakeGenericType(genericParamTypes);
                        goto case "Dictionary`2";

                    case "Dictionary`2":
                        collectionObject = Activator.CreateInstance(collectionType);

                        var collectionCount = Randomizer.RandomInt(1, MAX_RANDOM_COLLECTION_COUNT);
                        for (var i = 0; i < collectionCount; i++)
                        {
                            var childKey = InterrogateType(genericParamTypes[0], null, _iteration);
                            var childValue = InterrogateType(genericParamTypes[1], null, _iteration);
                            AddToCollection(ref collectionObject, childKey, childValue);
                        }
                        break;
                }
            }

            return collectionObject;
        }

        private static void AddToCollection<T>(ref T collectionObject, params object[] parmeterObjs)
        {
            //TODO: Accommodate for IEnumerable objects that don't have an Add method.
            var methInfo = collectionObject.GetType().GetMethod("Add");
            methInfo.Invoke(collectionObject, parmeterObjs);
        }

        private static void SetCollectionValue(ref object parentObject, object collectionObject,
            PropertyInfo propertyInfo)
        {
            //Attempt to directly set value.  If collection is read-only, than individual collection objects will have to be added one-by-one.
            try
            {
                propertyInfo.SetValue(parentObject
                    , collectionObject
                    , null);
            }
            catch
            {
                try
                {
                    Type parentType = parentObject.GetType();
                    Type collectionType = collectionObject.GetType();
                    Type[] genericArgs = collectionType.GetGenericArguments();
                    MethodInfo[] methInfoArray =
                        parentType.GetMethods(BindingFlags.Instance | BindingFlags.Public |
                                              BindingFlags.FlattenHierarchy);

                    var methInfo = methInfoArray.FirstOrDefault(m => m.Name == "Add" + genericArgs[0].Name);
                    if (methInfo == default(MethodInfo))
                    {
                        methInfo = methInfoArray.FirstOrDefault(m => m.Name.Contains("Add")
                                                                     && m.GetParameters().Count() == genericArgs.Count()
                                                                     &&
                                                                     m.GetParameters()
                                                                         .All(p => genericArgs.Contains(p.ParameterType)));
                    }
                    if (methInfo != default(MethodInfo))
                    {
                        foreach (object obj in collectionObject as IEnumerable)
                        {
                            methInfo.Invoke(parentObject, new[] {obj});
                        }
                    }
                }
                catch
                {
                }
            }
        }
    }
}