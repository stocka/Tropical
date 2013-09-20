﻿''' <summary>
''' Contains methods for validating items such as CSS class names and file names.
''' </summary>
Public Class Validation

  ''' <summary>
  ''' A regular expression for validating a CSS class name.
  ''' </summary>
  Private Shared ReadOnly CssClassNameRegex As New System.Text.RegularExpressions.Regex("-?[_a-zA-Z]+[_a-zA-Z0-9-]*")

  ''' <summary>
  ''' A set of file names reserved by the Windows operating system.
  ''' </summary>
  Private Shared ReadOnly ReservedFileNames As New HashSet(Of String)(StringComparer.InvariantCultureIgnoreCase) From {
    "CON", "PRN", "AUX", "NUL", "CLOCK$",
    "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
    "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"}

  ''' <summary>
  ''' Prevents a default instance of the <see cref="Validation"/> class from being created.
  ''' </summary>
  Private Sub New()
  End Sub

  ''' <summary>
  ''' Validates the provided CSS class name.
  ''' </summary>
  ''' <param name="className">The class name.</param>
  ''' <returns><c>true</c> if the provided CSS class name is valid, <c>false</c> otherwise.</returns>
  Public Shared Function ValidateCssClass(className As String) As Boolean
    Return CssClassNameRegex.IsMatch(className)
  End Function

  ''' <summary>
  ''' Validates the provided file path.
  ''' </summary>
  ''' <param name="filePath">The file path.</param>
  ''' <returns><c>true</c> if the provided file path is valid, <c>false</c> otherwise.</returns>
  Public Shared Function ValidateFilePath(filePath As String) As Boolean

    ' Handle null/whitespace path names
    If String.IsNullOrWhiteSpace(filePath) Then
      Return False
    End If

    ' Validate the entire path first, checking for length and invalid characters
    If filePath.Length > 260 Then
      Return False
    End If

    For Each c In System.IO.Path.GetInvalidPathChars()
      If filePath.Contains(c) Then
        Return False
      End If
    Next

    ' Validate the file name too if that's provided.
    Dim fileName As String = System.IO.Path.GetFileName(filePath)

    If Not String.IsNullOrWhiteSpace(fileName) Then
      Return ValidateFileName(fileName)
    Else
      Return True
    End If

  End Function

  ''' <summary>
  ''' Validates the provided file name.
  ''' </summary>
  ''' <param name="fileName">The file name.</param>
  ''' <returns><c>true</c> if the provided file name is valid, <c>false</c> otherwise.</returns>
  Public Shared Function ValidateFileName(fileName As String) As Boolean

    ' Handle null/whitespace file names
    If String.IsNullOrWhiteSpace(fileName) Then
      Return False
    End If

    ' Validate invalid file name characters.
    For Each c In System.IO.Path.GetInvalidFileNameChars()
      If fileName.Contains(c) Then
        Return False
      End If
    Next

    ' Get the filename without an extension and then see if it's reserved.
    Dim fileNameNoExt As String = System.IO.Path.GetFileNameWithoutExtension(fileName)

    If ReservedFileNames.Contains(fileNameNoExt.Trim) Then
      Return False
    End If

    Return True

  End Function

End Class
