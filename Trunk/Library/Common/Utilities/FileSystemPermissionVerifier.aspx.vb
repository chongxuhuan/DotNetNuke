Imports System.IO

Namespace DotNetNuke.Common.Utilities
    ''' <summary>
    ''' Verifies the abililty to create and delete files and folders
    ''' </summary>
    ''' <remarks>This class is not meant for use in modules, or in any other manner outside the DotNetNuke core.</remarks>
    Public Class FileSystemPermissionVerifier
        Private _basePath As String

        Public Sub New(ByVal basePath As String)
            _basePath = basePath
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' VerifyFileCreate checks whether a file can be created
        ''' </summary>
        ''' -----------------------------------------------------------------------------
        Public Function VerifyFileCreate() As Boolean
            Dim verifyPath As String = Path.Combine(_basePath, "Verify\Verify.txt")
            Dim verified As Boolean = VerifyFolderCreate()

            If verified Then
                'Attempt to create the File
                Try
                    If File.Exists(verifyPath) Then
                        File.Delete(verifyPath)
                    End If

                    Dim fileStream As Stream = File.Create(verifyPath)
                    fileStream.Close()

                Catch ex As Exception
                    verified = False
                End Try
            End If

            Return verified
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' VerifyFileDelete checks whether a file can be deleted
        ''' </summary>
        ''' -----------------------------------------------------------------------------
        Public Function VerifyFileDelete() As Boolean
            Dim verifyPath As String = Path.Combine(_basePath, "Verify\Verify.txt")
            Dim verified As Boolean = VerifyFileCreate()

            If verified Then
                'Attempt to delete the File
                Try
                    File.Delete(verifyPath)
                Catch ex As Exception
                    verified = False
                End Try
            End If

            Return verified
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' VerifyFolderCreate checks whether a folder can be created
        ''' </summary>
        ''' -----------------------------------------------------------------------------
        Public Function VerifyFolderCreate() As Boolean
            Dim verifyPath As String = Path.Combine(_basePath, "Verify")
            Dim verified As Boolean = True

            'Attempt to create the Directory
            Try
                If Directory.Exists(verifyPath) Then
                    Directory.Delete(verifyPath, True)
                End If

                Directory.CreateDirectory(verifyPath)
            Catch ex As Exception
                verified = False
            End Try

            Return verified
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' VerifyFolderDelete checks whether a folder can be deleted
        ''' </summary>
        ''' -----------------------------------------------------------------------------
        Public Function VerifyFolderDelete() As Boolean
            Dim verifyPath As String = Path.Combine(_basePath, "Verify")
            Dim verified As Boolean = VerifyFolderCreate()

            If verified Then
                'Attempt to delete the Directory
                Try
                    Directory.Delete(verifyPath)
                Catch ex As Exception
                    verified = False
                End Try
            End If

            Return verified
        End Function

        Public Function VerifyAll() As Boolean
            Return VerifyFileDelete() AndAlso VerifyFolderDelete()
        End Function
    End Class
End Namespace