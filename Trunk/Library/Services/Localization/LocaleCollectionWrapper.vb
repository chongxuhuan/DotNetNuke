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

Namespace DotNetNuke.Services.Localization

	''' -----------------------------------------------------------------------------
	''' <summary>
	''' The LocaleCollectionWrapper class provides a simple wrapper around a  
	''' <see cref="T:DotNetNuke.Services.Localization.LocaleCollection" />. 
	''' </summary>
	''' <remarks>
	''' The <see cref="T:DotNetNuke.Services.Localization.LocaleCollection" /> class
	''' implements a custom dictionary class which does not provide for simple databinding.
	''' This wrapper class exposes the individual objects of the underlying dictionary class
	''' thereby allowing for simple databinding to the colleciton of objects.
	''' </remarks>
	''' <history>
	''' 	[jbrinkman]	11/17/2004	Created
	''' </history>
	''' -----------------------------------------------------------------------------
    <Obsolete("Deprecated in DNN 5.0.")> _
    Public Class LocaleCollectionWrapper
        Implements IEnumerator, IEnumerable

        Private _locales As LocaleCollection
        Private _index As Integer = 0

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new instance of the <see cref="T:DotNetNuke.Services.Localization.LocaleCollectionWrapper" />
        '''  class containing the specified collection <see cref="T:DotNetNuke.Services.Localization.Locale" /> objects.
        ''' </summary>
        ''' <param name="Locales">A <see cref="T:DotNetNuke.Services.Localization.LocaleCollection" /> object 
        ''' which is wrapped by the collection. </param>
        ''' <remarks>This overloaded constructor copies the <see cref="T:DotNetNuke.ModuleAction" />s
        '''  from the indicated array.</remarks>
        ''' <history>
        ''' 	[jbrinkman]	11/17/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal Locales As LocaleCollection)
            _locales = Locales
        End Sub

        Public ReadOnly Property Current() As Object Implements System.Collections.IEnumerator.Current
            Get
                Return _locales(_locales.Keys.Item(_index))
            End Get
        End Property

        Public Function MoveNext() As Boolean Implements System.Collections.IEnumerator.MoveNext
            _index += 1
            Return _index < _locales.Keys.Count
        End Function

        Public Sub Reset() Implements System.Collections.IEnumerator.Reset
            _index = 0
        End Sub

        Public Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return CType(Me, IEnumerator)
        End Function
    End Class

End Namespace