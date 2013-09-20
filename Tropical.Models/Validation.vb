''' <summary>
''' Contains methods for validating items such as CSS class names and file names.
''' </summary>
Public Class Validation

  ''' <summary>
  ''' A regular expression for validating a CSS class name.
  ''' </summary>
  Private Shared ReadOnly CssClassNameRegex As New System.Text.RegularExpressions.Regex("-?[_a-zA-Z]+[_a-zA-Z0-9-]*")

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
  ''' Validates the provided file name.
  ''' </summary>
  ''' <param name="fileName">The file name.</param>
  ''' <returns><c>true</c> if the provided file name is valid, <c>false</c> otherwise.</returns>
  Public Shared Function ValidateFileName(fileName As String) As Boolean

    For Each c In System.IO.Path.GetInvalidFileNameChars()
      If fileName.Contains(c) Then
        Return False
      End If
    Next

    Return True

  End Function

End Class
