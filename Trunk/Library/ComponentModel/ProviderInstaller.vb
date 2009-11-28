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

Imports DotNetNuke.Framework.Providers
Imports System.Configuration

Imports System.Collections.Generic

Namespace DotNetNuke.ComponentModel

    Public Class ProviderInstaller
        Implements IComponentInstaller

        ' TODO: Rewrite Provider Loading code in here instead of calling the old code

        Private _ComponentLifeStyle As ComponentLifeStyleType
        Private _ProviderInterface As Type
        Private _ProviderType As String

        Public Sub New(ByVal providerType As String, ByVal providerInterface As Type)
            Me._ComponentLifeStyle = ComponentLifeStyleType.Singleton
            Me._ProviderType = providerType
            Me._ProviderInterface = providerInterface
        End Sub

        Public Sub New(ByVal providerType As String, ByVal providerInterface As Type, ByVal lifeStyle As ComponentLifeStyleType)
            Me._ComponentLifeStyle = lifeStyle
            Me._ProviderType = providerType
            Me._ProviderInterface = providerInterface
        End Sub

        Public Sub InstallComponents(ByVal container As IContainer) Implements IComponentInstaller.InstallComponents
            Dim config As ProviderConfiguration = ProviderConfiguration.GetProviderConfiguration(_ProviderType)
            ' Register the default provider first (so it is the first component registered for its service interface
            If config IsNot Nothing Then
                InstallProvider(container, DirectCast(config.Providers(config.DefaultProvider), Provider))

                ' Register the others
                For Each provider As Provider In config.Providers.Values
                    ' Skip the default because it was registered above
                    If Not config.DefaultProvider.Equals(provider.Name, StringComparison.OrdinalIgnoreCase) Then
                        InstallProvider(container, provider)
                    End If
                Next
            End If
        End Sub

        Private Sub InstallProvider(ByVal container As IContainer, ByVal provider As Provider)
            If provider IsNot Nothing Then

                ' Get the provider type
                Dim type As Type = System.Web.Compilation.BuildManager.GetType(provider.Type, False, True)
                If type Is Nothing Then
                    Throw New ConfigurationErrorsException(String.Format("Could not load provider {0}", provider.Type))
                End If

                ' Register the component
                container.RegisterComponent(provider.Name, _ProviderInterface, type, _ComponentLifeStyle)

                ' Load the settings into a dictionary
                Dim settingsDict As New Dictionary(Of String, String)
                settingsDict.Add("providerName", provider.Name)
                For Each key As String In provider.Attributes.Keys
                    settingsDict.Add(key, provider.Attributes.Get(key))
                Next

                ' Register the settings as dependencies
                container.RegisterComponentSettings(type.FullName, settingsDict)
            End If
        End Sub

    End Class

End Namespace
