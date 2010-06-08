Imports System.Web.Script.Serialization

Namespace DotNetNuke.Common.Utilities
    ''' <summary>
    ''' Json Extensions based on the JavaScript Serializer in System.web
    ''' </summary>
    Public Module JsonExtensionsWeb

        ''' <summary> 
        ''' Serializes a type to Json. Note the type must be marked Serializable 
        ''' or include a DataContract attribute. 
        ''' </summary> 
        ''' <param name="value"></param> 
        ''' <returns></returns>
        Public Function ToJsonString(ByVal value As Object) As String
            Dim ser As New JavaScriptSerializer()
            Dim json As String = ser.Serialize(value)
            Return json
        End Function

        ''' <summary> 
        ''' Extension method on object that serializes the value to Json. 
        ''' Note the type must be marked Serializable or include a DataContract attribute. 
        ''' </summary> 
        ''' <param name="value"></param> 
        ''' <returns></returns> 
        <System.Runtime.CompilerServices.Extension()> _
        Public Function ToJson(ByVal value As Object) As String
            Return ToJsonString(value)
        End Function

        ''' <summary> 
        ''' Deserializes a json string into a specific type. 
        ''' Note that the type specified must be serializable. 
        ''' </summary> 
        ''' <param name="json"></param> 
        ''' <param name="type"></param> 
        ''' <returns></returns> 
        Public Function FromJsonString(ByVal json As String, ByVal type As Type) As Object
            ' *** Have to use Reflection with a 'dynamic' non constant type instance 
            Dim ser As New System.Web.Script.Serialization.JavaScriptSerializer()

            Dim result As Object = ser.[GetType]().GetMethod("Deserialize").MakeGenericMethod(type).Invoke(ser, New Object(0) {json})
            Return result
        End Function

        ''' <summary> 
        ''' Extension method to string that deserializes a json string 
        ''' into a specific type. 
        ''' Note that the type specified must be serializable. 
        ''' </summary> 
        ''' <param name="json"></param> 
        ''' <param name="type"></param> 
        ''' <returns></returns> 
        <System.Runtime.CompilerServices.Extension()> _
        Public Function FromJson(ByVal json As String, ByVal type As Type) As Object
            Return FromJsonString(json, type)
        End Function

        <System.Runtime.CompilerServices.Extension()> _
        Public Function FromJson(Of TType)(ByVal json As String) As TType
            Dim ser As New System.Web.Script.Serialization.JavaScriptSerializer()

            Dim result As TType = ser.Deserialize(Of TType)(json)
            Return result
        End Function
    End Module
End Namespace