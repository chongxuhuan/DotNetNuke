Namespace DotNetNuke.Application
    Public Enum ReleaseMode
        ''' <summary>
        ''' Not asssigned
        ''' </summary>
        None
        ''' <summary>
        ''' Alpha release
        ''' </summary>
        Alpha
        ''' <summary>
        ''' Beta release
        ''' </summary>
        Beta
        ''' <summary>
        ''' Release candidate
        ''' </summary>
        RC
        ''' <summary>
        ''' Stable release version
        ''' </summary>
        Stable
    End Enum
    <AttributeUsage(AttributeTargets.Assembly)> _
    Public Class AssemblyStatusAttribute
        Inherits System.Attribute

#Region "Fields"
        Private _releaseMode As ReleaseMode
#End Region

#Region "Constructors"
        ''' <summary>
        ''' Initializes a new instance of the <see cref="AssemblyStatusAttribute" /> class.
        ''' </summary>
        ''' <param name="releaseMode">The release mode.</param>
        Public Sub New(ByVal releaseMode As ReleaseMode)
            _releaseMode = releaseMode
        End Sub
#End Region

#Region "Properties"
        ''' <summary>
        ''' Gets the status.
        ''' </summary>
        ''' <value>The status, related to the <c>Enum ReleaseMode</c>:
        ''' <code>None
        ''' Alpha
        ''' Beta
        ''' RC
        ''' Stable
        ''' </code></value>
        Public ReadOnly Property Status() As ReleaseMode
            Get
                Return _releaseMode
            End Get
        End Property
#End Region

    End Class
End Namespace
