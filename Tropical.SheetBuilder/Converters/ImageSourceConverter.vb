Imports System.Windows.Interop

''' <summary>
''' A custom value converter for image sources, which will fall back
''' to &quot;no image&quot; or &quot;missing image&quot; placeholders
''' as appropriate.
''' </summary>
Public Class ImageSourceConverter
  Implements IValueConverter

  Private Shared ReadOnly NoImageSource As BitmapImage
  Private Shared ReadOnly MissingImageSource As BitmapImage

  ''' <summary>
  ''' Initializes the <see cref="ImageSourceConverter"/> class.
  ''' </summary>
  Shared Sub New()

    Using noImageStream As IO.Stream =
      Reflection.Assembly.GetExecutingAssembly.GetManifestResourceStream("Tropical.SheetBuilder.NoImage.png")

      NoImageSource = New BitmapImage()
      NoImageSource.BeginInit()
      NoImageSource.StreamSource = noImageStream
      NoImageSource.EndInit()

    End Using

    Using missingImageStream As IO.Stream =
      Reflection.Assembly.GetExecutingAssembly.GetManifestResourceStream("Tropical.SheetBuilder.MissingImage.png")

      MissingImageSource = New BitmapImage()
      MissingImageSource.BeginInit()
      MissingImageSource.StreamSource = missingImageStream
      MissingImageSource.EndInit()

    End Using

  End Sub

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

    ' If it's null/whitespace, use the "No Image" image
    If value Is Nothing OrElse String.IsNullOrWhiteSpace(value.ToString) Then
      Return NoImageSource
    End If

    ' If it's a good path, it exists, return it.
    If Models.Validation.ValidateFilePath(value) AndAlso System.IO.File.Exists(value) Then
      Return value
    Else
      Return MissingImageSource
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
