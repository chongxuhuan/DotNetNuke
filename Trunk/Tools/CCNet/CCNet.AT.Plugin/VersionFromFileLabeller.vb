Imports ThoughtWorks.CruiseControl.Core
Imports ThoughtWorks.CruiseControl.Core.Util


<Exortech.NetReflector.ReflectorType("VersionFromFileLabeller")> _
Public Class VersionFromFileLabeller
	Implements ThoughtWorks.CruiseControl.Core.ILabeller

	Private Const VersionRegEx As String = "<Assembly:\ AssemblyVersion\(""(?<Version>\d+\.\d+\.\d+\.\d+)""\)>"

	<Exortech.NetReflector.ReflectorProperty("DefaultVersion", Required:=True)> _
	Public DefaultVersion As String = "1.0.0.0"

	<Exortech.NetReflector.ReflectorProperty("FilePath", Required:=True)> _
	Public FilePath As String

	<Exortech.NetReflector.ReflectorProperty("IncrementOnFailure", Required:=False)> _
	Public IncrementOnFailure As Boolean = True





	Public Function Generate(ByVal resultFromLastBuild As ThoughtWorks.CruiseControl.Core.IIntegrationResult) As String Implements ThoughtWorks.CruiseControl.Core.ILabeller.Generate

		If resultFromLastBuild Is Nothing OrElse resultFromLastBuild.IsInitial() _
		 OrElse System.IO.File.Exists(GetFilePath(resultFromLastBuild, FilePath)) = False Then
			Return DefaultVersion
		End If

		Dim retValue As String = ""
		Dim content As String = System.IO.File.ReadAllText(GetFilePath(resultFromLastBuild, FilePath))
		Dim matches As System.Text.RegularExpressions.MatchCollection = System.Text.RegularExpressions.Regex.Matches(content, VersionRegEx)

		For Each match As System.Text.RegularExpressions.Match In matches
			retValue = match.Groups("Version").Value

			If resultFromLastBuild.Succeeded OrElse IncrementOnFailure Then
				Dim values() As String = retValue.Split("."c)

				values(3) = (CInt(values(3)) + 1).ToString()

				retValue = String.Format("{0}.{1}.{2}.{3}", values(0), values(1), values(2), values(3))
			End If
		Next

		Return retValue
	End Function

	Public Sub Run(ByVal result As ThoughtWorks.CruiseControl.Core.IIntegrationResult) Implements ThoughtWorks.CruiseControl.Core.ITask.Run
		result.Label = Generate(result)
	End Sub




	Private Function GetFilePath(ByVal resultFromLastBuild As ThoughtWorks.CruiseControl.Core.IIntegrationResult, ByVal path As String) As String
		If System.IO.Path.IsPathRooted(path) Then Return path
		Return System.IO.Path.Combine(path, resultFromLastBuild.WorkingDirectory)
	End Function

End Class
