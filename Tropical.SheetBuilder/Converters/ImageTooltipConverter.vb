''' <summary>
''' A custom value converter for getting a tooltip for
''' an image that may not be specified or may not exist.
''' </summary>
Public Class ImageTooltipConverter
  Implements IValueConverter

  ''' <summary>
  ''' Converts a value.
  ''' </summary>
  ''' <param name="value">The value produced by the binding source.</param>
  ''' <param name="targetType">The type of the binding target property.</param>
  ''' <param name="parameter">The converter parameter to use.</param>
  ''' <param name="culture">The culture to use in the converter.</param>
  ''' <returns>
  ''' A converted value. If the method returns null, the valid null value is used.
  ''' </returns>
  Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert

    ' If it's null/whitespace, indicate we haven't specified something.
    If value Is Nothing OrElse String.IsNullOrWhiteSpace(value.ToString) Then
      Return "No image has been specified."
    End If

    ' If it's a good path and it exists, don't throw up a tooltip
    If Models.Validation.ValidateFilePath(value) AndAlso System.IO.File.Exists(value) Then
      Return DependencyProperty.UnsetValue
    Else
      Return "The path to the image is invalid or the image does not exist."
    End If

  End Function

  ''' <summary>
  ''' Converts a value.
  ''' </summary>
  ''' <param name="value">The value that is produced by the binding target.</param>
  ''' <param name="targetType">The type to convert to.</param>
  ''' <param name="parameter">The converter parameter to use.</param>
  ''' <param name="culture">The culture to use in the converter.</param>
  ''' <returns>
  ''' A converted value. If the method returns null, the valid null value is used.
  ''' </returns>
  ''' <exception cref="System.NotImplementedException">This method is not implemented.</exception>
  Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
    Throw New NotImplementedException()
  End Function

End Class
