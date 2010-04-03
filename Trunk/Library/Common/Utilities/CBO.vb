'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2010
' by DotNetNuke Corporation
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'

Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Reflection
Imports DotNetNuke.Entities.Modules
Imports System.Xml.Serialization
Imports System.Xml
Imports System.Text
Imports DotNetNuke.Common.Utilities.DataCache
Imports System.Xml.XPath
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.Common.Utilities

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.Common.Utilities
    ''' Class:      CBO
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The CBO class generates objects.
    ''' </summary>
    ''' <history>
    '''     [cnurse]	12/01/2007	Documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class CBO

#Region "Private Constants"

        Private Const defaultCacheByProperty As String = "ModuleID"
        Private Const defaultCacheTimeOut As Integer = 20
        Private Const defaultPrimaryKey As String = "ItemID"

        Private Const objectMapCacheKey As String = "ObjectMap_"
        Private Const schemaCacheKey As String = "Schema_"

#End Region

#Region "Private Shared Methods"

#Region "Object Creation/Hydration Helper Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CreateObjectFromReader creates an object of a specified type from the 
        ''' provided DataReader
        ''' </summary>
        ''' <param name="objType">The type of the Object</param>
        ''' <param name="dr">The IDataReader to use to fill the object</param>
        ''' <param name="closeReader">A flag that indicates whether the DataReader should be closed</param>
        ''' <returns>The object (TObject)</returns>
        ''' <history>
        ''' 	[cnurse]	11/30/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function CreateObjectFromReader(ByVal objType As Type, ByVal dr As IDataReader, ByVal closeReader As Boolean) As Object
            Dim objObject As Object = Nothing
            Dim isSuccess As Boolean = Null.NullBoolean
            Dim canRead As Boolean = True

            If closeReader Then
                canRead = False
                ' read datareader
                If dr.Read() Then
                    canRead = True
                End If
            End If

            Try
                If canRead Then
                    'Create the Object
                    objObject = CreateObject(objType, False)

                    'hydrate the custom business object
                    FillObjectFromReader(objObject, dr)
                End If
                isSuccess = True
            Finally
                ' Ensure DataReader is closed
                If (Not isSuccess) Then closeReader = True
                CloseDataReader(dr, closeReader)
            End Try

            Return objObject
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FillDictionaryFromReader fills a dictionary of objects of a specified type 
        ''' from a DataReader.
        ''' </summary>
        ''' <typeparam name="TKey">The type of the key</typeparam>
        ''' <typeparam name="TValue">The type of the value</typeparam>
        ''' <param name="keyField">The key field for the object.  This is used as 
        ''' the key in the Dictionary.</param>
        ''' <param name="dr">The IDataReader to use to fill the objects</param>
        ''' <param name="objDictionary">The Dictionary to fill.</param>
        ''' <returns>A Dictionary of objects (T)</returns>
        ''' <history>
        ''' 	[cnurse]	11/30/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function FillDictionaryFromReader(Of TKey, TValue)(ByVal keyField As String, ByVal dr As IDataReader, ByVal objDictionary As IDictionary(Of TKey, TValue)) As IDictionary(Of TKey, TValue)
            Dim objObject As TValue
            Dim keyValue As TKey

            Try
                ' iterate datareader
                While dr.Read
                    'Create the Object
                    objObject = DirectCast(CreateObjectFromReader(GetType(TValue), dr, False), TValue)

                    If keyField = "KeyID" AndAlso TypeOf objObject Is IHydratable Then
                        'Get the value of the key field from the KeyID
                        keyValue = CType(Null.SetNull(DirectCast(objObject, IHydratable).KeyID, keyValue), TKey)
                    Else
                        'Get the value of the key field from the DataReader
                        If GetType(TKey).Name = "Int32" And dr(keyField).GetType.Name = "Decimal" Then
                            keyValue = CType(Null.SetNull(dr(keyField), keyValue), TKey)
                        ElseIf GetType(TKey).Name.ToLower = "string" And dr(keyField).GetType.Name.ToLower = "dbnull" Then
                            keyValue = CType(Null.SetNull(dr(keyField), ""), TKey)
                        Else
                            keyValue = DirectCast(Null.SetNull(dr(keyField), keyValue), TKey)
                        End If
                    End If


                    ' add to dictionary
                    If objObject IsNot Nothing Then
                        objDictionary(keyValue) = objObject
                    End If
                End While
            Finally
                ' Ensure DataReader is closed
                CloseDataReader(dr, True)
            End Try

            'Return the dictionary
            Return objDictionary
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FillListFromReader fills a list of objects of a specified type 
        ''' from a DataReader
        ''' </summary>
        ''' <param name="objType">The type of the business object</param>
        ''' <param name="dr">The IDataReader to use to fill the objects</param>
        ''' <param name="objList">The List to Fill</param>
        ''' <param name="closeReader">A flag that indicates whether the DataReader should be closed</param>
        ''' <returns>A List of objects (TItem)</returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 	[cnurse]	11/30/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function FillListFromReader(ByVal objType As Type, ByVal dr As IDataReader, ByVal objList As IList, ByVal closeReader As Boolean) As IList
            Dim objObject As Object
            Dim isSuccess As Boolean = Null.NullBoolean

            Try
                ' iterate datareader
                While dr.Read
                    'Create the Object
                    objObject = CreateObjectFromReader(objType, dr, False)

                    ' add to collection
                    objList.Add(objObject)
                End While
                isSuccess = True
            Finally
                ' Ensure DataReader is closed
                If (Not isSuccess) Then closeReader = True
                CloseDataReader(dr, closeReader)
            End Try

            Return objList
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FillListFromReader fills a list of objects of a specified type 
        ''' from a DataReader
        ''' </summary>
        ''' <typeparam name="TItem">The type of the business object</typeparam>
        ''' <param name="dr">The IDataReader to use to fill the objects</param>
        ''' <param name="objList">The List to Fill</param>
        ''' <param name="closeReader">A flag that indicates whether the DataReader should be closed</param>
        ''' <returns>A List of objects (TItem)</returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 	[cnurse]	11/30/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function FillListFromReader(Of TItem)(ByVal dr As IDataReader, ByVal objList As IList(Of TItem), ByVal closeReader As Boolean) As IList(Of TItem)
            Dim objObject As TItem
            Dim isSuccess As Boolean = Null.NullBoolean

            Try
                ' iterate datareader
                While dr.Read
                    'Create the Object
                    objObject = DirectCast(CreateObjectFromReader(GetType(TItem), dr, False), TItem)

                    ' add to collection
                    objList.Add(objObject)
                End While
                isSuccess = True
            Finally
                ' Ensure DataReader is closed
                If (Not isSuccess) Then closeReader = True
                CloseDataReader(dr, closeReader)
            End Try

            Return objList
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FillObjectFromReader fills an object from the provided DataReader.  If the object 
        ''' implements the IHydratable interface it will use the object's IHydratable.Fill() method.
        ''' Otherwise, it will use reflection to fill the object.
        ''' </summary>
        ''' <param name="objObject">The object to fill</param>
        ''' <param name="dr">The DataReader</param>
        ''' <history>
        ''' 	[cnurse]	11/30/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Sub FillObjectFromReader(ByVal objObject As Object, ByVal dr As IDataReader)
            Try
                'Determine if object is IHydratable
                If TypeOf objObject Is IHydratable Then
                    'Use IHydratable's Fill
                    Dim objHydratable As IHydratable = TryCast(objObject, IHydratable)
                    If objHydratable IsNot Nothing Then
                        objHydratable.Fill(dr)
                    End If
                Else
                    'Use Reflection
                    HydrateObject(objObject, dr)
                End If
            Catch iex As IndexOutOfRangeException
                'Call to GetOrdinal is being made with a bad column name
                If Host.ThrowCBOExceptions Then
                    Throw New ObjectHydrationException("Error Reading DataReader", iex, objObject.GetType(), dr)
                Else
                    Exceptions.LogException(iex)
                End If
            End Try

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' HydrateObject uses reflection to hydrate an object.
        ''' </summary>
        ''' <param name="objObject">The object to Hydrate</param>
        ''' <param name="dr">The IDataReader that contains the columns of data for the object</param>
        ''' <history>
        ''' 	[cnurse]	11/29/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Sub HydrateObject(ByVal objObject As Object, ByVal dr As IDataReader)
            Dim objPropertyInfo As PropertyInfo = Nothing
            Dim objPropertyType As Type = Nothing
            Dim objDataValue As Object
            Dim objDataType As Type
            Dim intIndex As Integer

            ' get cached object mapping for type
            Dim objMappingInfo As ObjectMappingInfo = GetObjectMapping(objObject.GetType)

            If TypeOf objObject Is DotNetNuke.Entities.BaseEntityInfo AndAlso Not TypeOf objObject Is DotNetNuke.Services.Scheduling.ScheduleItem Then
                'Call the base classes fill method to populate base class properties
                CType(objObject, DotNetNuke.Entities.BaseEntityInfo).FillBaseProperties(dr)
            End If

            ' fill object with values from datareader
            For intIndex = 0 To dr.FieldCount - 1
                'If the Column matches a Property in the Object Map's PropertyInfo Dictionary
                If objMappingInfo.Properties.TryGetValue(dr.GetName(intIndex).ToUpperInvariant, objPropertyInfo) Then
                    'Get its type
                    objPropertyType = objPropertyInfo.PropertyType

                    'If property can be set
                    If objPropertyInfo.CanWrite Then
                        'Get the Data Value from the data reader
                        objDataValue = dr.GetValue(intIndex)

                        'Get the Data Value's type
                        objDataType = objDataValue.GetType

                        If IsDBNull(objDataValue) Then
                            ' set property value to Null
                            objPropertyInfo.SetValue(objObject, Null.SetNull(objPropertyInfo), Nothing)
                        ElseIf objPropertyType.Equals(objDataType) Then
                            'Property and data objects are the same type
                            objPropertyInfo.SetValue(objObject, objDataValue, Nothing)
                        Else
                            ' business object info class member data type does not match datareader member data type

                            'need to handle enumeration conversions differently than other base types
                            If objPropertyType.BaseType.Equals(GetType(System.Enum)) Then
                                ' check if value is numeric and if not convert to integer ( supports databases like Oracle )
                                If IsNumeric(objDataValue) Then
                                    objPropertyInfo.SetValue(objObject, System.Enum.ToObject(objPropertyType, Convert.ToInt32(objDataValue)), Nothing)
                                Else
                                    objPropertyInfo.SetValue(objObject, System.Enum.ToObject(objPropertyType, objDataValue), Nothing)
                                End If
                            ElseIf objPropertyType Is GetType(Guid) Then
                                ' guid is not a datatype common across all databases ( ie. Oracle )
                                objPropertyInfo.SetValue(objObject, Convert.ChangeType(New Guid(objDataValue.ToString()), objPropertyType), Nothing)
                            ElseIf objPropertyType Is GetType(System.Version) Then
                                objPropertyInfo.SetValue(objObject, New Version(objDataValue.ToString()), Nothing)
                            ElseIf (objPropertyType Is objDataType) Then
                                ' try explicit conversion
                                objPropertyInfo.SetValue(objObject, objDataValue, Nothing)
                            Else
                                objPropertyInfo.SetValue(objObject, Convert.ChangeType(objDataValue, objPropertyType), Nothing)
                            End If

                        End If
                    End If
                End If
            Next
        End Sub


#End Region

#Region "Object Mapping Helper Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetCacheByProperty gets the name of the Property that the object is cached by.
        ''' For most modules this would be the "ModuleID".  In the core Tabs are cached 
        ''' by "PortalID" and Modules are cached by "TabID".
        ''' </summary>
        ''' <param name="objType">The type of the business object</param>
        ''' <returns>The name of the Property</returns>
        ''' <history>
        ''' 	[cnurse]	11/30/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function GetCacheByProperty(ByVal objType As Type) As String
            Dim cacheProperty As String = defaultCacheByProperty

            'check if a Cache Property Attribute has been set
            'Dim cachePropertyAttributes As Object() = objType.GetCustomAttributes(GetType(CachePropertyAttribute), True)
            'If (cachePropertyAttributes.Length > 0) Then
            '    Dim cachePropertyAttrib As CachePropertyAttribute = CType(cachePropertyAttributes(0), CachePropertyAttribute)
            '    cacheProperty = cachePropertyAttrib.CacheProperty
            'End If

            Return cacheProperty
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetCacheTimeOutMultiplier gets the multiplier to use in the sliding cache 
        ''' expiration.  This value is multiplier by the Host Performance setting, which
        ''' in turn can be set by the Host Account.
        ''' </summary>
        ''' <param name="objType">The type of the business object</param>
        ''' <returns>The timeout expiry multiplier</returns>
        ''' <history>
        ''' 	[cnurse]	12/01/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function GetCacheTimeOutMultiplier(ByVal objType As Type) As Integer
            Dim cacheTimeOut As Integer = defaultCacheTimeOut

            ''check if a Cache TimeOut Attribute has been used
            'Dim cacheTimeOutAttributes As Object() = objType.GetCustomAttributes(GetType(CacheTimeOutAttribute), True)
            'If (cacheTimeOutAttributes.Length > 0) Then
            '    Dim cacheTimeOutAttrib As CacheTimeOutAttribute = CType(cacheTimeOutAttributes(0), CacheTimeOutAttribute)
            '    cacheTimeOut = cacheTimeOutAttrib.TimeOut
            'End If

            Return cacheTimeOut
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetColumnName gets the name of the Database Column that maps to the property.
        ''' </summary>
        ''' <param name="objProperty">The proeprty of the business object</param>
        ''' <returns>The name of the Database Column</returns>
        ''' <history>
        ''' 	[cnurse]	12/02/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function GetColumnName(ByVal objProperty As PropertyInfo) As String
            Dim columnName As String = objProperty.Name

            'check if a ColumnName Attribute has been used
            'Dim columnNameAttributes As Object() = objProperty.GetCustomAttributes(GetType(ColumnNameAttribute), True)
            'If (columnNameAttributes.Length > 0) Then
            '    Dim columnNameAttrib As ColumnNameAttribute = CType(columnNameAttributes(0), ColumnNameAttribute)
            '    columnName = columnNameAttrib.ColumnName
            'End If

            Return columnName
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetObjectMapping gets an instance of the ObjectMappingInfo class for the type.
        ''' This is cached using a high priority as reflection is expensive.
        ''' </summary>
        ''' <param name="objType">The type of the business object</param>
        ''' <returns>An ObjectMappingInfo object representing the mapping for the object</returns>
        ''' <history>
        ''' 	[cnurse]	12/01/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function GetObjectMapping(ByVal objType As Type) As ObjectMappingInfo
            Dim cacheKey As String = objectMapCacheKey + objType.FullName
            Dim objMap As ObjectMappingInfo = CType(DataCache.GetCache(cacheKey), ObjectMappingInfo)

            If objMap Is Nothing Then
                'Create an ObjectMappingInfo instance
                objMap = New ObjectMappingInfo()
                objMap.ObjectType = objType.FullName

                'Reflect on class to create Object Map
                'objMap.CacheByProperty = GetCacheByProperty(objType)
                'objMap.CacheTimeOutMultiplier = GetCacheTimeOutMultiplier(objType)
                objMap.PrimaryKey = GetPrimaryKey(objType)
                objMap.TableName = GetTableName(objType)

                'Iterate through the objects properties and add each one to the ObjectMappingInfo's Properties Dictionary 
                For Each objProperty As PropertyInfo In objType.GetProperties()
                    objMap.Properties.Add(objProperty.Name.ToUpperInvariant, objProperty)
                    objMap.ColumnNames.Add(objProperty.Name.ToUpperInvariant, GetColumnName(objProperty))
                Next
                'Persist to Cache
                DataCache.SetCache(cacheKey, objMap)
            End If

            'Return Object Map
            Return objMap
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetPrimaryKey gets the Primary Key property
        ''' </summary>
        ''' <param name="objType">The type of the business object</param>
        ''' <returns>The name of the Primary Key property</returns>
        ''' <history>
        ''' 	[cnurse]	12/01/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function GetPrimaryKey(ByVal objType As Type) As String
            Dim primaryKey As String = defaultPrimaryKey

            'First check if a TableName Attribute has been used
            'Dim primaryKeyAttributes As Object() = objType.GetCustomAttributes(GetType(PrimaryKeyAttribute), True)
            'If (primaryKeyAttributes.Length > 0) Then
            '    Dim primaryKeyAttrib As PrimaryKeyAttribute = CType(primaryKeyAttributes(0), PrimaryKeyAttribute)
            '    primaryKey = primaryKeyAttrib.PrimaryKey
            'End If

            Return primaryKey
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetTableName gets the name of the Database Table that maps to the object.
        ''' </summary>
        ''' <param name="objType">The type of the business object</param>
        ''' <returns>The name of the Database Table</returns>
        ''' <history>
        ''' 	[cnurse]	11/30/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function GetTableName(ByVal objType As Type) As String
            Dim tableName As String = String.Empty

            ''First check if a TableName Attribute has been used
            'Dim tableNameAttributes As Object() = objType.GetCustomAttributes(GetType(TableNameAttribute), True)
            'If (tableNameAttributes.Length > 0) Then
            '    Dim tableNameAttrib As TableNameAttribute = CType(tableNameAttributes(0), TableNameAttribute)
            '    tableName = tableNameAttrib.TableName
            'End If

            ' If no attrubute then use Type Name
            If String.IsNullOrEmpty(tableName) Then
                tableName = objType.Name

                If tableName.EndsWith("Info") Then
                    'Remove Info ending
                    tableName.Replace("Info", String.Empty)
                End If
            End If

            'Check if there is an object qualifier
            If Not String.IsNullOrEmpty(Config.GetSetting("ObjectQualifier")) Then
                tableName = Config.GetSetting("ObjectQualifier") + tableName
            End If

            Return tableName
        End Function

#End Region

#End Region

#Region "Public Shared Methods"

#Region "Clone Object"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CloneObject clones an object
        ''' </summary>
        ''' <param name="objObject">The Object to Clone</param>
        ''' <history>
        ''' 	[cnurse]	11/29/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CloneObject(ByVal objObject As Object) As Object
            Try
                Dim objType As Type = objObject.GetType()
                Dim objNewObject As Object = Activator.CreateInstance(objType)

                ' get cached object mapping for type
                Dim objMappingInfo As ObjectMappingInfo = GetObjectMapping(objType)

                For Each kvp As KeyValuePair(Of String, PropertyInfo) In objMappingInfo.Properties
                    Dim objProperty As PropertyInfo = kvp.Value

                    If objProperty.CanWrite Then

                        'Check if property is ICloneable
                        Dim objPropertyClone As ICloneable = TryCast(objProperty.GetValue(objObject, Nothing), ICloneable)
                        If objPropertyClone Is Nothing Then
                            objProperty.SetValue(objNewObject, objProperty.GetValue(objObject, Nothing), Nothing)
                        Else
                            objProperty.SetValue(objNewObject, objPropertyClone.Clone(), Nothing)
                        End If

                        'Check if Property is IEnumerable
                        Dim enumerable As IEnumerable = TryCast(objProperty.GetValue(objObject, Nothing), IEnumerable)
                        If enumerable IsNot Nothing Then
                            Dim list As IList = TryCast(objProperty.GetValue(objNewObject, Nothing), IList)
                            If list IsNot Nothing Then
                                For Each obj As Object In enumerable
                                    list.Add(CloneObject(obj))
                                Next obj
                            End If

                            Dim dic As IDictionary = TryCast(objProperty.GetValue(objNewObject, Nothing), IDictionary)
                            If dic IsNot Nothing Then
                                For Each de As DictionaryEntry In enumerable
                                    dic.Add(de.Key, CloneObject(de.Value))
                                Next de
                            End If
                        End If
                    End If
                Next
                Return objNewObject
            Catch exc As Exception
                LogException(exc)
                Return Nothing
            End Try
        End Function

#End Region

#Region "CloseDataReader"

        Public Shared Sub CloseDataReader(ByVal dr As IDataReader, ByVal closeReader As Boolean)
            ' close datareader
            If Not dr Is Nothing AndAlso closeReader Then
                dr.Close()
            End If
        End Sub

#End Region

#Region "Create Object"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CreateObject creates a new object of Type TObject.
        ''' </summary>
        ''' <typeparam name="TObject">The type of object to create.</typeparam>
        ''' <remarks>This overload does not initialise the object</remarks>
        ''' <history>
        ''' 	[cnurse]	11/30/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CreateObject(Of TObject)() As TObject
            Return DirectCast(CreateObject(GetType(TObject), False), TObject)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CreateObject creates a new object of Type TObject.
        ''' </summary>
        ''' <typeparam name="TObject">The type of object to create.</typeparam>
        ''' <param name="initialise">A flag that indicates whether to initialise the
        ''' object.</param>
        ''' <history>
        ''' 	[cnurse]	11/30/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CreateObject(Of TObject)(ByVal initialise As Boolean) As TObject
            Return DirectCast(CreateObject(GetType(TObject), initialise), TObject)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CreateObject creates a new object.
        ''' </summary>
        ''' <param name="objType">The type of object to create.</param>
        ''' <param name="initialise">A flag that indicates whether to initialise the
        ''' object.</param>
        ''' <history>
        ''' 	[cnurse]	11/30/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CreateObject(ByVal objType As Type, ByVal initialise As Boolean) As Object
            Dim objObject As Object = Activator.CreateInstance(objType)

            If initialise Then
                InitializeObject(objObject)
            End If

            Return objObject
        End Function

#End Region

#Region "DeserializeObject"

        Public Shared Function DeserializeObject(Of TObject)(ByVal fileName As String) As TObject
            Return DeserializeObject(Of TObject)(XmlReader.Create(New FileStream(fileName, FileMode.Open, FileAccess.Read)))
        End Function

        Public Shared Function DeserializeObject(Of TObject)(ByVal document As XmlDocument) As TObject
            Return DeserializeObject(Of TObject)(XmlReader.Create(New StringReader(document.OuterXml)))
        End Function

        Public Shared Function DeserializeObject(Of TObject)(ByVal stream As Stream) As TObject
            Return DeserializeObject(Of TObject)(XmlReader.Create(stream))
        End Function

        Public Shared Function DeserializeObject(Of TObject)(ByVal reader As TextReader) As TObject
            Return DeserializeObject(Of TObject)(XmlReader.Create(reader))
        End Function

        Public Shared Function DeserializeObject(Of TObject)(ByVal reader As XmlReader) As TObject
            'First Create the Object
            Dim objObject As TObject = CreateObject(Of TObject)(True)

            'Try to cast the Object as IXmlSerializable
            Dim xmlSerializableObject As IXmlSerializable = TryCast(objObject, IXmlSerializable)

            If xmlSerializableObject Is Nothing Then
                'Use XmlSerializer
                Dim serializer As New XmlSerializer(objObject.GetType())

                objObject = DirectCast(serializer.Deserialize(reader), TObject)
            Else
                'Use XmlReader
                xmlSerializableObject.ReadXml(reader)
            End If

            Return objObject
        End Function

#End Region

#Region "FillCollection"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FillCollection fills a Collection of objects from a DataReader
        ''' </summary>
        ''' <param name="dr">The Data Reader</param>
        ''' <param name="objType">The type of the Object</param>
        ''' <history>
        ''' 	[cnurse]	11/29/2007	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function FillCollection(ByVal dr As IDataReader, ByVal objType As Type) As ArrayList
            Return DirectCast(FillListFromReader(objType, dr, New ArrayList(), True), ArrayList)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FillCollection fills a Collection of objects from a DataReader
        ''' </summary>
        ''' <param name="dr">The Data Reader</param>
        ''' <param name="objType">The type of the Object</param>
        ''' <param name="closeReader">Flag that indicates whether the Data Reader should be closed.</param>
        ''' <history>
        ''' 	[cnurse]	11/29/2007	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function FillCollection(ByVal dr As IDataReader, ByVal objType As Type, ByVal closeReader As Boolean) As ArrayList
            Return DirectCast(FillListFromReader(objType, dr, New ArrayList(), closeReader), ArrayList)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FillCollection fills a Collection of objects from a DataReader
        ''' </summary>
        ''' <param name="dr">The Data Reader</param>
        ''' <param name="objType">The type of the Object</param>
        ''' <param name="objToFill">An IList to fill</param>
        ''' <history>
        ''' 	[cnurse]	11/29/2007	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function FillCollection(ByVal dr As IDataReader, ByVal objType As Type, ByRef objToFill As IList) As IList
            Return FillListFromReader(objType, dr, objToFill, True)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FillCollection fills a Collection of objects from a DataReader
        ''' </summary>
        ''' <typeparam name="TItem">The type of object</typeparam>
        ''' <param name="dr">The Data Reader</param>
        ''' <history>
        ''' 	[cnurse]	11/29/2007	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function FillCollection(Of TItem)(ByVal dr As IDataReader) As List(Of TItem)
            Return DirectCast(FillListFromReader(Of TItem)(dr, New List(Of TItem), True), List(Of TItem))
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FillCollection fills a Collection of objects from a DataReader
        ''' </summary>
        ''' <typeparam name="TItem">The type of object</typeparam>
        ''' <param name="objToFill">The List to fill</param>
        ''' <param name="dr">The Data Reader</param>
        ''' <history>
        ''' 	[cnurse]	11/29/2007	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function FillCollection(Of TItem)(ByVal dr As IDataReader, ByRef objToFill As IList(Of TItem)) As IList(Of TItem)
            Return FillListFromReader(Of TItem)(dr, objToFill, True)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FillCollection fills a List of objects from a DataReader
        ''' </summary>
        ''' <typeparam name="TItem">The type of the Object</typeparam>
        ''' <param name="objToFill">The List to fill</param>
        ''' <param name="dr">The Data Reader</param>
        ''' <param name="closeReader">A flag that indicates whether the DataReader should be closed</param>
        ''' <history>
        ''' 	[cnurse]	11/29/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function FillCollection(Of TItem)(ByVal dr As IDataReader, ByVal objToFill As IList(Of TItem), ByVal closeReader As Boolean) As IList(Of TItem)
            Return FillListFromReader(Of TItem)(dr, objToFill, closeReader)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Generic version of FillCollection fills a List custom business object of a specified type 
        ''' from the supplied DataReader
        ''' </summary>
        ''' <param name="dr">The IDataReader to use to fill the object</param>
        ''' <param name="objType">The type of the Object</param>
        ''' <param name="totalRecords">The total No of records</param>
        ''' <returns>A List of custom business objects</returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 	[cnurse]	01/28/2008	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function FillCollection(ByVal dr As IDataReader, ByRef objType As Type, ByRef totalRecords As Integer) As ArrayList
            Dim objFillCollection As ArrayList = DirectCast(FillListFromReader(objType, dr, New ArrayList(), False), ArrayList)
            Try
                If dr.NextResult Then
                    'Get the total no of records from the second result
                    totalRecords = Globals.GetTotalRecords(dr)
                End If
            Catch exc As Exception
                Exceptions.LogException(exc)
            Finally
                ' Ensure DataReader is closed
                CloseDataReader(dr, True)
            End Try

            Return objFillCollection
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Generic version of FillCollection fills a List custom business object of a specified type 
        ''' from the supplied DataReader
        ''' </summary>
        ''' <typeparam name="T">The type of the business object</typeparam>
        ''' <param name="dr">The IDataReader to use to fill the object</param>
        ''' <returns>A List of custom business objects</returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 	[cnurse]	10/10/2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function FillCollection(Of T)(ByVal dr As IDataReader, ByRef totalRecords As Integer) As List(Of T)
            Dim objFillCollection As IList(Of T) = FillCollection(Of T)(dr, New List(Of T), False)
            Try
                If dr.NextResult Then
                    'Get the total no of records from the second result
                    totalRecords = Globals.GetTotalRecords(dr)
                End If
            Catch exc As Exception
                Exceptions.LogException(exc)
            Finally
                ' Ensure DataReader is closed
                CloseDataReader(dr, True)
            End Try

            Return DirectCast(objFillCollection, List(Of T))
        End Function

#End Region

#Region "FillDictionary"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FillDictionary fills a Dictionary of objects from a DataReader
        ''' </summary>
        ''' <typeparam name="TItem">The value for the Dictionary Item</typeparam>
        ''' <param name="dr">The Data Reader</param>
        ''' <history>
        ''' 	[cnurse]	11/29/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function FillDictionary(Of TItem As IHydratable)(ByVal dr As IDataReader) As IDictionary(Of Integer, TItem)
            Return FillDictionaryFromReader(Of Integer, TItem)("KeyID", dr, New Dictionary(Of Integer, TItem))
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FillDictionary fills a Dictionary of objects from a DataReader
        ''' </summary>
        ''' <typeparam name="TItem">The value for the Dictionary Item</typeparam>
        ''' <param name="objToFill">The Dictionary to fill</param>
        ''' <param name="dr">The Data Reader</param>
        ''' <history>
        ''' 	[cnurse]	11/29/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function FillDictionary(Of TItem As IHydratable)(ByVal dr As IDataReader, ByRef objToFill As IDictionary(Of Integer, TItem)) As IDictionary(Of Integer, TItem)
            Return FillDictionaryFromReader(Of Integer, TItem)("KeyID", dr, objToFill)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FillDictionary fills a Dictionary of objects from a DataReader
        ''' </summary>
        ''' <typeparam name="TKey">The key for the Dictionary</typeparam>
        ''' <typeparam name="TValue">The value for the Dictionary Item</typeparam>
        ''' <param name="keyField">The key field used for the Key</param>
        ''' <param name="dr">The Data Reader</param>
        ''' <history>
        ''' 	[cnurse]	11/29/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function FillDictionary(Of TKey, TValue)(ByVal keyField As String, ByVal dr As IDataReader) As Dictionary(Of TKey, TValue)
            Return DirectCast(FillDictionaryFromReader(Of TKey, TValue)(keyField, dr, New Dictionary(Of TKey, TValue)), Dictionary(Of TKey, TValue))
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FillDictionary fills a Dictionary of objects from a DataReader
        ''' </summary>
        ''' <typeparam name="TKey">The key for the Dictionary</typeparam>
        ''' <typeparam name="TValue">The value for the Dictionary Item</typeparam>
        ''' <param name="keyField">The key field used for the Key</param>
        ''' <param name="objDictionary">The Dictionary to fill</param>
        ''' <param name="dr">The Data Reader</param>
        ''' <history>
        ''' 	[cnurse]	11/29/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function FillDictionary(Of TKey, TValue)(ByVal keyField As String, ByVal dr As IDataReader, ByVal objDictionary As IDictionary(Of TKey, TValue)) As Dictionary(Of TKey, TValue)
            Return DirectCast(FillDictionaryFromReader(Of TKey, TValue)(keyField, dr, objDictionary), Dictionary(Of TKey, TValue))
        End Function

#End Region

#Region "FillObject"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FillObject fills an object from a DataReader
        ''' </summary>
        ''' <typeparam name="TObject">The type of the object</typeparam>
        ''' <param name="dr">The Data Reader</param>
        ''' <history>
        ''' 	[cnurse]	11/29/2007	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function FillObject(Of TObject)(ByVal dr As IDataReader) As TObject
            Return DirectCast(CreateObjectFromReader(GetType(TObject), dr, True), TObject)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FillObject fills an object from a DataReader
        ''' </summary>
        ''' <typeparam name="TObject">The type of the object</typeparam>
        ''' <param name="dr">The Data Reader</param>
        ''' <param name="closeReader">A flag that indicates the reader should be closed</param>
        ''' <history>
        ''' 	[cnurse]	11/29/2007	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function FillObject(Of TObject)(ByVal dr As IDataReader, ByVal closeReader As Boolean) As TObject
            Return DirectCast(CreateObjectFromReader(GetType(TObject), dr, closeReader), TObject)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FillObject fills an object from a DataReader
        ''' </summary>
        ''' <param name="dr">The Data Reader</param>
        ''' <param name="objType">The type of the object</param>
        ''' <history>
        ''' 	[cnurse]	11/29/2007	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function FillObject(ByVal dr As IDataReader, ByVal objType As Type) As Object
            Return CreateObjectFromReader(objType, dr, True)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FillObject fills an object from a DataReader
        ''' </summary>
        ''' <param name="dr">The Data Reader</param>
        ''' <param name="objType">The type of the object</param>
        ''' <param name="closeReader">A flag that indicates the reader should be closed</param>
        ''' <history>
        ''' 	[cnurse]	11/29/2007	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function FillObject(ByVal dr As IDataReader, ByVal objType As Type, ByVal closeReader As Boolean) As Object
            Return CreateObjectFromReader(objType, dr, closeReader)
        End Function

#End Region

        Public Shared Function FillQueryable(Of TItem)(ByVal dr As IDataReader) As IQueryable(Of TItem)
            Return FillListFromReader(Of TItem)(dr, New List(Of TItem), True).AsQueryable()
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FillSortedList fills a SortedList of objects from a DataReader
        ''' </summary>
        ''' <typeparam name="TKey">The key for the SortedList</typeparam>
        ''' <typeparam name="TValue">The value for the SortedList Item</typeparam>
        ''' <param name="keyField">The key field used for the Key</param>
        ''' <param name="dr">The Data Reader</param>
        ''' <history>
        ''' 	[cnurse]	11/29/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function FillSortedList(Of TKey, TValue)(ByVal keyField As String, ByVal dr As IDataReader) As SortedList(Of TKey, TValue)
            Return DirectCast(FillDictionaryFromReader(Of TKey, TValue)(keyField, dr, New SortedList(Of TKey, TValue)), SortedList(Of TKey, TValue))
        End Function

#Region "GetCachedObject"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetCachedObject gets an object from the Cache
        ''' </summary>
        ''' <typeparam name="TObject">The type of th object to fetch</typeparam>
        ''' <param name="cacheItemArgs">A CacheItemArgs object that provides parameters to manage the
        ''' cache AND to fetch the item if the cache has expired</param>
        ''' <param name="cacheItemExpired">A CacheItemExpiredCallback delegate that is used to repopulate
        ''' the cache if the item has expired</param>
        ''' <history>
        ''' 	[cnurse]	01/13/2008	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetCachedObject(Of TObject)(ByVal cacheItemArgs As CacheItemArgs, ByVal cacheItemExpired As CacheItemExpiredCallback) As TObject
            Return DataCache.GetCachedData(Of TObject)(cacheItemArgs, cacheItemExpired)
        End Function

        Public Shared Function GetCachedObject(Of TObject)(ByVal cacheItemArgs As CacheItemArgs, ByVal cacheItemExpired As CacheItemExpiredCallback, ByVal saveInDictionary As Boolean) As TObject
            Return DataCache.GetCachedData(Of TObject)(cacheItemArgs, cacheItemExpired, saveInDictionary)
        End Function

#End Region

#Region "GetProperties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetProperties gets a Dictionary of the Properties for an object
        ''' </summary>
        ''' <typeparam name="TObject">The type of the object</typeparam>
        ''' <history>
        ''' 	[cnurse]	01/17/2008	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetProperties(Of TObject)() As Dictionary(Of String, PropertyInfo)
            Return GetObjectMapping(GetType(TObject)).Properties
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetProperties gets a Dictionary of the Properties for an object
        ''' </summary>
        ''' <param name="objType">The type of the object</param>
        ''' <history>
        ''' 	[cnurse]	01/17/2008	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetProperties(ByVal objType As Type) As Dictionary(Of String, PropertyInfo)
            Return GetObjectMapping(objType).Properties
        End Function

#End Region

#Region "InitializeObject"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' InitializeObject initialises all the properties of an object to their 
        ''' Null Values.
        ''' </summary>
        ''' <param name="objObject">The object to Initialise</param>
        ''' <history>
        ''' 	[cnurse]	11/29/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub InitializeObject(ByVal objObject As Object)
            ' initialize properties
            For Each objPropertyInfo As PropertyInfo In GetObjectMapping(objObject.GetType).Properties.Values
                If objPropertyInfo.CanWrite Then
                    objPropertyInfo.SetValue(objObject, Null.SetNull(objPropertyInfo), Nothing)
                End If
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' InitializeObject initialises all the properties of an object to their 
        ''' Null Values.
        ''' </summary>
        ''' <param name="objObject">The object to Initialise</param>
        ''' <param name="objType">The type of the object</param>
        ''' <history>
        ''' 	[cnurse]	11/29/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function InitializeObject(ByVal objObject As Object, ByVal objType As Type) As Object
            ' initialize properties
            For Each objPropertyInfo As PropertyInfo In GetObjectMapping(objType).Properties.Values
                If objPropertyInfo.CanWrite Then
                    objPropertyInfo.SetValue(objObject, Null.SetNull(objPropertyInfo), Nothing)
                End If
            Next

            Return objObject
        End Function

#End Region

#Region "SerializeObject"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' SerializeObject serializes an Object
        ''' </summary>
        ''' <param name="objObject">The object to Initialise</param>
        ''' <param name="fileName">A filename for the resulting serialized xml</param>
        ''' <history>
        ''' 	[cnurse]	01/17/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub SerializeObject(ByVal objObject As Object, ByVal fileName As String)
            Using writer As XmlWriter = XmlWriter.Create(fileName, XmlUtils.GetXmlWriterSettings(ConformanceLevel.Fragment))
                SerializeObject(objObject, writer)
                writer.Flush()
            End Using
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' SerializeObject serializes an Object
        ''' </summary>
        ''' <param name="objObject">The object to Initialise</param>
        ''' <param name="document">An XmlDocument to serialize to</param>
        ''' <history>
        ''' 	[cnurse]	01/17/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub SerializeObject(ByVal objObject As Object, ByVal document As XmlDocument)
            Dim sb As New StringBuilder()

            'Serialize the object
            SerializeObject(objObject, XmlWriter.Create(sb, XmlUtils.GetXmlWriterSettings(ConformanceLevel.Document)))

            'Load XmlDocument
            document.LoadXml(sb.ToString())
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' SerializeObject serializes an Object
        ''' </summary>
        ''' <param name="objObject">The object to Initialise</param>
        ''' <param name="stream">A Stream to serialize to</param>
        ''' <history>
        ''' 	[cnurse]	01/17/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub SerializeObject(ByVal objObject As Object, ByVal stream As Stream)
            Using writer As XmlWriter = XmlWriter.Create(stream, XmlUtils.GetXmlWriterSettings(ConformanceLevel.Fragment))
                SerializeObject(objObject, writer)
                writer.Flush()
            End Using
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' SerializeObject serializes an Object
        ''' </summary>
        ''' <param name="objObject">The object to Initialise</param>
        ''' <param name="textWriter">A TextWriter to serialize to</param>
        ''' <history>
        ''' 	[cnurse]	01/17/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub SerializeObject(ByVal objObject As Object, ByVal textWriter As TextWriter)
            Using writer As XmlWriter = XmlWriter.Create(textWriter, XmlUtils.GetXmlWriterSettings(ConformanceLevel.Fragment))
                SerializeObject(objObject, writer)
                writer.Flush()
            End Using
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' SerializeObject serializes an Object
        ''' </summary>
        ''' <param name="objObject">The object to Initialise</param>
        ''' <param name="writer">An XmlWriter to serialize to</param>
        ''' <history>
        ''' 	[cnurse]	01/17/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub SerializeObject(ByVal objObject As Object, ByVal writer As XmlWriter)
            'Try to cast the Object as IXmlSerializable
            Dim xmlSerializableObject As IXmlSerializable = TryCast(objObject, IXmlSerializable)

            If xmlSerializableObject Is Nothing Then
                'Use XmlSerializer
                Dim serializer As New XmlSerializer(objObject.GetType())

                serializer.Serialize(writer, objObject)
            Else
                'Use XmlWriter
                xmlSerializableObject.WriteXml(writer)
            End If
        End Sub

#End Region

#End Region

#Region "Obsolete"

        <Obsolete("Obsolete in DotNetNuke 5.0.  Replaced by GetProperties(Of TObject)() ")> _
        Public Shared Function GetPropertyInfo(ByVal objType As Type) As ArrayList
            Dim arrProperties As New ArrayList()

            ' get cached object mapping for type
            Dim objMappingInfo As ObjectMappingInfo = GetObjectMapping(objType)

            arrProperties.AddRange(objMappingInfo.Properties.Values())

            Return arrProperties
        End Function

        <Obsolete("Obsolete in DotNetNuke 5.0.  Replaced by SerializeObject(Object) ")> _
        Public Shared Function Serialize(ByVal objObject As Object) As XmlDocument
            Dim document As New XmlDocument
            SerializeObject(objObject, document)
            Return document
        End Function

#End Region

    End Class


End Namespace
