''' <summary>
''' A converter for converting between integers and strings.
''' </summary>
Public Class IntConverter
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

    Dim intVal As Int32

    If Int32.TryParse(value.ToString(), intVal) Then
      Return intVal
    Else
      Return 0
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
  Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack

    If value Is Nothing OrElse String.IsNullOrWhiteSpace(value.ToString()) Then
      Return "0"
    Else
      Return value.ToString()
    End If

  End Function

End Class
