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

Namespace DotNetNuke.Entities.Modules.Actions

	'''-----------------------------------------------------------------------------
	''' Project		: DotNetNuke
	''' Class		: ModuleActionCollection
	'''-----------------------------------------------------------------------------
	''' <summary>
	''' Represents a collection of <see cref="T:DotNetNuke.ModuleAction" /> objects.
	''' </summary>
	''' <remarks>The ModuleActionCollection is a custom collection of ModuleActions.
	''' Each ModuleAction in the collection has it's own <see cref="P:DotNetNuke.ModuleAction.Actions" />
	'''  collection which provides the ability to create a heirarchy of ModuleActions.</remarks>
	''' <history>
	''' 	[Joe] 	10/9/2003	Created
	''' </history>
	'''-----------------------------------------------------------------------------
	Public Class ModuleActionCollection
		Inherits CollectionBase

#Region "Constructors"

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new, empty instance of the <see cref="T:DotNetNuke.ModuleActionCollection" /> class.
        ''' </summary>
        ''' <remarks>The default constructor creates an empty collection of <see cref="T:DotNetNuke.ModuleAction" />
        '''  objects.</remarks>
        ''' <history>
        ''' 	[Joe] 	10/9/2003	Created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Sub New()
            MyBase.New()
        End Sub

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new instance of the <see cref="T:DotNetNuke.ModuleActionCollection" />
        '''  class containing the elements of the specified source collection.
        ''' </summary>
        ''' <param name="value">A <see cref="T:DotNetNuke.ModuleActionCollection" /> with which to initialize the collection.</param>
        ''' <remarks>This overloaded constructor copies the <see cref="T:DotNetNuke.ModuleAction" />s
        '''  from the indicated collection.</remarks>
        ''' <history>
        ''' 	[Joe] 	10/9/2003	Created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Sub New(ByVal value As ModuleActionCollection)
            MyBase.New()
            AddRange(value)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new instance of the <see cref="T:DotNetNuke.ModuleActionCollection" />
        '''  class containing the specified array of <see cref="T:DotNetNuke.ModuleAction" /> objects.
        ''' </summary>
        ''' <param name="value">An array of <see cref="T:DotNetNuke.ModuleAction" /> objects 
        ''' with which to initialize the collection. </param>
        ''' <remarks>This overloaded constructor copies the <see cref="T:DotNetNuke.ModuleAction" />s
        '''  from the indicated array.</remarks>
        ''' <history>
        ''' 	[Joe] 	10/9/2003	Created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Sub New(ByVal value As ModuleAction())
            MyBase.New()
            AddRange(value)
        End Sub

#End Region

#Region "Default Property"

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the <see cref="T:DotNetNuke.ModuleActionCollection" /> at the 
        ''' specified index in the collection.
        ''' <para>
        ''' In VB.Net, this property is the indexer for the <see cref="T:DotNetNuke.ModuleActionCollection" /> class.
        ''' </para>
        ''' </summary>
        ''' <param name="index">The index of the collection to access.</param>
        ''' <value>A <see cref="T:DotNetNuke.ModuleAction" /> at each valid index.</value>
        ''' <remarks>This method is an indexer that can be used to access the collection.</remarks>
        ''' <history>
        ''' 	[Joe] 	10/9/2003	Created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Default Public Property Item(ByVal index As Integer) As ModuleAction
            Get
                Return CType(List(index), ModuleAction)
            End Get
            Set(ByVal Value As ModuleAction)
                List(index) = Value
            End Set
        End Property

#End Region

#Region "Public Methods"

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Add an element of the specified <see cref="T:DotNetNuke.ModuleAction" /> to the end of the collection.
        ''' </summary>
        ''' <param name="value">An object of type <see cref="T:DotNetNuke.ModuleAction" /> to add to the collection.</param>
        ''' <returns>The index of the newly added <see cref="T:DotNetNuke.ModuleAction" /></returns>
        ''' <history>
        ''' 	[Joe] 	10/9/2003	Created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Function Add(ByVal value As ModuleAction) As Integer
            Return List.Add(value)
        End Function

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Add an element of the specified <see cref="T:DotNetNuke.ModuleAction" /> to the end of the collection.
        ''' </summary>
        ''' <param name="ID">This is the identifier to use for this action.</param>
        ''' <param name="Title">This is the title that will be displayed for this action</param>
        ''' <param name="CmdName">The command name passed to the client when this action is 
        ''' clicked.</param>
        ''' <param name="CmdArg">The command argument passed to the client when this action is 
        ''' clicked.</param>
        ''' <param name="Icon">The URL of the Icon to place next to this action</param>
        ''' <param name="Url">The destination URL to redirect the client browser when this 
        ''' action is clicked.</param>
        ''' <param name="UseActionEvent">Determines whether client will receive an event
        ''' notification</param>
        ''' <param name="Secure">The security access level required for access to this action</param>
        ''' <param name="Visible">Whether this action will be displayed</param>
        ''' <returns>The index of the newly added <see cref="T:DotNetNuke.ModuleAction" /></returns>
        ''' <remarks>This method creates a new <see cref="T:DotNetNuke.ModuleAction" /> with the specified
        ''' values, adds it to the collection and returns the index of the newly created ModuleAction.</remarks>
        ''' <history>
        ''' 	[Joe] 	10/18/2003	Created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Function Add(ByVal ID As Integer, ByVal Title As String, ByVal CmdName As String, Optional ByVal CmdArg As String = "", Optional ByVal Icon As String = "", Optional ByVal Url As String = "", Optional ByVal UseActionEvent As Boolean = False, Optional ByVal Secure As SecurityAccessLevel = SecurityAccessLevel.Anonymous, Optional ByVal Visible As Boolean = True, Optional ByVal NewWindow As Boolean = False) As ModuleAction
            Return Me.Add(ID, Title, CmdName, CmdArg, Icon, Url, "", UseActionEvent, Secure, Visible, NewWindow)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Add an element of the specified <see cref="T:DotNetNuke.ModuleAction" /> to the end of the collection.
        ''' </summary>
        ''' <param name="ID">This is the identifier to use for this action.</param>
        ''' <param name="Title">This is the title that will be displayed for this action</param>
        ''' <param name="CmdName">The command name passed to the client when this action is 
        ''' clicked.</param>
        ''' <param name="CmdArg">The command argument passed to the client when this action is 
        ''' clicked.</param>
        ''' <param name="Icon">The URL of the Icon to place next to this action</param>
        ''' <param name="Url">The destination URL to redirect the client browser when this 
        ''' action is clicked.</param>
        ''' <param name="ClientScript">Client side script to be run when the this action is 
        ''' clicked.</param>
        ''' <param name="UseActionEvent">Determines whether client will receive an event
        ''' notification</param>
        ''' <param name="Secure">The security access level required for access to this action</param>
        ''' <param name="Visible">Whether this action will be displayed</param>
        ''' <returns>The index of the newly added <see cref="T:DotNetNuke.ModuleAction" /></returns>
        ''' <remarks>This method creates a new <see cref="T:DotNetNuke.ModuleAction" /> with the specified
        ''' values, adds it to the collection and returns the index of the newly created ModuleAction.</remarks>
        '''         ''' <history>
        ''' 	[jbrinkman]	5/22/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function Add(ByVal ID As Integer, ByVal Title As String, ByVal CmdName As String, ByVal CmdArg As String, ByVal Icon As String, ByVal Url As String, ByVal ClientScript As String, ByVal UseActionEvent As Boolean, ByVal Secure As SecurityAccessLevel, ByVal Visible As Boolean, ByVal NewWindow As Boolean) As ModuleAction
            Dim ModAction As ModuleAction = New ModuleAction(ID, Title, CmdName, CmdArg, Icon, Url, ClientScript, UseActionEvent, Secure, Visible, NewWindow)
            Me.Add(ModAction)
            Return ModAction
        End Function

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Copies the elements of the specified <see cref="T:DotNetNuke.ModuleAction" />
        '''  array to the end of the collection.
        ''' </summary>
        ''' <param name="value">An array of type <see cref="T:DotNetNuke.ModuleAction" />
        '''  containing the objects to add to the collection.</param>
        ''' <history>
        ''' 	[Joe] 	10/9/2003	Created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Sub AddRange(ByVal value As ModuleAction())
            Dim i As Integer
            For i = 0 To value.Length - 1
                Add(value(i))
            Next i
        End Sub

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Adds the contents of another <see cref="T:DotNetNuke.ModuleActionCollection" />
        '''  to the end of the collection.
        ''' </summary>
        ''' <param name="value">A <see cref="T:DotNetNuke.ModuleActionCollection" /> containing 
        ''' the objects to add to the collection. </param>
        ''' <history>
        ''' 	[Joe] 	10/9/2003	Created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Sub AddRange(ByVal value As ModuleActionCollection)
            Dim mA As ModuleAction
            For Each mA In value
                Add(mA)
            Next
        End Sub

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a value indicating whether the collection contains the specified <see cref="T:DotNetNuke.ModuleAction" />.
        ''' </summary>
        ''' <param name="value">The <see cref="T:DotNetNuke.ModuleAction" /> to search for in the collection.</param>
        ''' <returns><b>true</b> if the collection contains the specified object; otherwise, <b>false</b>.</returns>
        ''' <example>
        ''' <code>
        ''' ' Tests for the presence of a ModuleAction in the 
        ''' ' collection, and retrieves its index if it is found.
        ''' Dim testModuleAction = New ModuleAction(5, "Edit Action", "Edit")
        ''' Dim itemIndex As Integer = -1
        ''' If collection.Contains(testModuleAction) Then
        '''    itemIndex = collection.IndexOf(testModuleAction)
        ''' End If
        ''' </code>
        ''' </example>
        ''' <history>
        ''' 	[Joe] 	10/9/2003	Created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Function Contains(ByVal value As ModuleAction) As Boolean
            ' If value is not of type ModuleAction, this will return false.
            Return List.Contains(value)
        End Function

        Public Function GetActionByCommandName(ByVal name As String) As ModuleAction
            Dim retAction As ModuleAction = Nothing

            'Check each action in the List
            For Each modAction As ModuleAction In List
                If modAction.CommandName = name Then
                    retAction = modAction
                    Exit For
                End If

                'If action has children check them
                If modAction.HasChildren Then
                    Dim childAction As ModuleAction = modAction.Actions.GetActionByCommandName(name)
                    If childAction IsNot Nothing Then
                        retAction = childAction
                        Exit For
                    End If
                End If
            Next
            Return retAction
        End Function

        Public Function GetActionsByCommandName(ByVal name As String) As ModuleActionCollection
            Dim retActions As ModuleActionCollection = New ModuleActionCollection()

            'Check each action in the List
            For Each modAction As ModuleAction In List
                If modAction.CommandName = name Then
                    retActions.Add(modAction)
                End If

                'If action has children check them
                If modAction.HasChildren Then
                    retActions.AddRange(modAction.Actions.GetActionsByCommandName(name))
                End If
            Next
            Return retActions
        End Function

        Public Function GetActionByID(ByVal id As Integer) As ModuleAction
            Dim retAction As ModuleAction = Nothing

            'Check each action in the List
            For Each modAction As ModuleAction In List
                If modAction.ID = id Then
                    retAction = modAction
                    Exit For
                End If

                'If action has children check them
                If modAction.HasChildren Then
                    Dim childAction As ModuleAction = modAction.Actions.GetActionByID(id)
                    If childAction IsNot Nothing Then
                        retAction = childAction
                        Exit For
                    End If
                End If
            Next
            Return retAction
        End Function

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the index in the collection of the specified <see cref="T:DotNetNuke.ModuleActionCollection" />, 
        ''' if it exists in the collection.
        ''' </summary>
        ''' <param name="value">The <see cref="T:DotNetNuke.ModuleAction" /> to locate in the collection.</param>
        ''' <returns>The index in the collection of the specified object, if found; otherwise, -1.</returns>
        ''' <example> This example tests for the presense of a ModuleAction in the
        ''' collection, and retrieves its index if it is found.
        ''' <code>
        '''   Dim testModuleAction = New ModuleAction(5, "Edit Action", "Edit")
        '''   Dim itemIndex As Integer = -1
        '''   If collection.Contains(testModuleAction) Then
        '''     itemIndex = collection.IndexOf(testModuleAction)
        '''   End If
        ''' </code>
        ''' </example>
        ''' <history>
        ''' 	[Joe] 	10/9/2003	Created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Function IndexOf(ByVal value As ModuleAction) As Integer
            Return List.IndexOf(value)
        End Function

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Add an element of the specified <see cref="T:DotNetNuke.ModuleAction" /> to the 
        ''' collection at the designated index.
        ''' </summary>
        ''' <param name="index">An <see cref="T:system.int32">Integer</see> to indicate the location to add the object to the collection.</param>
        ''' <param name="value">An object of type <see cref="T:DotNetNuke.ModuleAction" /> to add to the collection.</param>
        ''' <example>
        ''' <code>
        ''' ' Inserts a ModuleAction at index 0 of the collection. 
        ''' collection.Insert(0, New ModuleAction(5, "Edit Action", "Edit"))
        ''' </code>
        ''' </example>
        ''' <history>
        ''' 	[Joe] 	10/9/2003	Created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Sub Insert(ByVal index As Integer, ByVal value As ModuleAction)
            List.Insert(index, value)
        End Sub

        '''----------------------------------------------------------------------------- 
        ''' <summary>
        ''' Remove the specified object of type <see cref="T:DotNetNuke.ModuleAction" /> from the collection.
        ''' </summary>
        ''' <param name="value">An object of type <see cref="T:DotNetNuke.ModuleAction" /> to remove from the collection.</param>
        ''' <example>
        ''' <code>
        ''' ' Removes the specified ModuleAction from the collection. 
        ''' Dim testModuleAction = New ModuleAction(5, "Edit Action", "Edit")
        ''' collection.Remove(testModuleAction)
        ''' </code>
        ''' </example>
        ''' <history>
        ''' 	[Joe] 	10/9/2003	Created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Sub Remove(ByVal value As ModuleAction)
            List.Remove(value)
        End Sub

#End Region

    End Class

End Namespace
