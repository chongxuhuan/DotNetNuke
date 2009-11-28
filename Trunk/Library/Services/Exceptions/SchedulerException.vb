'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2009
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

Imports System.Runtime.Serialization
Imports System.Security.Permissions

Namespace DotNetNuke.Services.Exceptions
	<Serializable()> Public Class SchedulerException
		Inherits BasePortalException

		' default constructor
		Public Sub New()
			MyBase.New()
		End Sub

		'constructor with exception message
		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub

		' constructor with message and inner exception
		Public Sub New(ByVal message As String, ByVal inner As Exception)
			MyBase.New(message, inner)
		End Sub

		Protected Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext)
			MyBase.New(info, context)
		End Sub


		<SecurityPermission(SecurityAction.Demand, SerializationFormatter:=True)> _
		Public Overrides Sub GetObjectData(ByVal info As SerializationInfo, ByVal context As StreamingContext)
			' Serialize this class' state and then call the base class GetObjectData
			MyBase.GetObjectData(info, context)
		End Sub

	End Class

End Namespace