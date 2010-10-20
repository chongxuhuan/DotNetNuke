Namespace DotNetNuke.Application
    Public Enum ReleaseMode
        None
        Alpha
        Beta
        RC
        Stable
    End Enum

    <AttributeUsage(AttributeTargets.Assembly)> _
    Public Class AssemblyStatusAttribute
        Inherits System.Attribute

#Region "Fields"
        Private _releaseMode As ReleaseMode
#End Region

#Region "Constructors"
        Public Sub New(ByVal releaseMode As ReleaseMode)
            _releaseMode = releaseMode
        End Sub
#End Region

#Region "Properties"
        Public ReadOnly Property Status() As ReleaseMode
            Get
                Return _releaseMode
            End Get
        End Property
#End Region

    End Class
End Namespace
