Namespace DotNetNuke.UI.WebControls.Design
    Public Class DefaultDesignerInfo
        Public ReadOnly Property DefaultName() As String
            Get
                Return "DotNetNuke"
            End Get
        End Property

        Public ReadOnly Property DefaultValue() As Integer
            Get
                Return 2006
            End Get
        End Property

        Public ReadOnly Property DefaultEnabled() As Boolean
            Get
                Return True
            End Get
        End Property

        Public ReadOnly Property DefaultEnum() As DesignerTest
            Get
                Return DesignerTest.EnumValue1
            End Get
        End Property
    End Class

    Public Enum DesignerTest
        EnumValue1 = 0
        Enumvalue2
    End Enum
End Namespace
