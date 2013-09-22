''' <summary>
''' A sprite sheet.
''' </summary>
<Serializable()>
Public Class SpriteSheet
  Implements IValidatableObject

  ''' <summary>
  ''' Gets or sets the name of the CSS class
  ''' that will be used for all sprites.
  ''' </summary>
  ''' <value>
  ''' The name of the CSS class that will
  ''' be used for all sprites.
  ''' </value>
  <Required(ErrorMessage:="The base CSS class name is required.")>
  Public Property BaseClassName As String

  ''' <summary>
  ''' Gets or sets the base filename that will
  ''' be used to generate the sprite sheet and associated
  ''' CSS stylesheet.
  ''' </summary>
  ''' <value>
  ''' The base filename that will be used to generate
  ''' the sprite sheet and associated CSS stylesheet.
  ''' </value>
  <Required(ErrorMessage:="The base file name is required.")>
  Public Property BaseFileName As String

  ''' <summary>
  ''' Gets or sets the dimensions for each image in the
  ''' sprite sheet.
  ''' </summary>
  ''' <value>
  ''' The dimensions for each image in the sprite sheet.
  ''' </value>
  Public Property ImageDimensions As System.Drawing.Size

  ''' <summary>
  ''' Gets or sets the sprites in the sheet.
  ''' </summary>
  ''' <value>
  ''' The sprites in the sheet.
  ''' </value>
  Public Property Sprites As New List(Of Sprite)

  ''' <summary>
  ''' Determines whether the specified object is valid.
  ''' </summary>
  ''' <param name="validationContext">The validation context.</param>
  ''' <returns>
  ''' A collection that holds failed-validation information.
  ''' </returns>
  Public Function Validate(validationContext As ValidationContext) As IEnumerable(Of ValidationResult) Implements IValidatableObject.Validate

    Dim brokenRules As New List(Of ValidationResult)

    ' Validate the CSS class name
    If Not String.IsNullOrWhiteSpace(Me.BaseClassName) AndAlso Not Validation.ValidateCssClass(Me.BaseClassName) Then
      brokenRules.Add(New ValidationResult("The specified base CSS class name is invalid.", {"BaseClassName"}))
    End If

    ' Validate the file name
    If Not String.IsNullOrWhiteSpace(Me.BaseFileName) AndAlso Not Validation.ValidateFileName(Me.BaseFileName) Then
      brokenRules.Add(New ValidationResult("The specified base file name is invalid.", {"BaseFileName"}))
    End If

    ' Validate the image dimensions
    If Me.ImageDimensions.Height <= 0 Then
      brokenRules.Add(New ValidationResult("The specified image height is invalid.", {"ImageDimensions.Height"}))
    End If

    If Me.ImageDimensions.Width <= 0 Then
      brokenRules.Add(New ValidationResult("The specified image width is invalid.", {"ImageDimensions.Width"}))
    End If

    ' Now validate the sprite collection.
    If Me.Sprites Is Nothing Then
      brokenRules.Add(New ValidationResult("The sprites collection is null.", {"Sprites"}))
    ElseIf Me.Sprites.Any() Then

      For Each spr In Me.Sprites

        Dim sprValidation As IEnumerable(Of ValidationResult) = spr.Validate(validationContext)

        ' Is this sprite valid?
        If sprValidation IsNot Nothing AndAlso sprValidation.Any() Then

          ' No. Build an error message.
          Dim sprValidationMessage As String
          Dim errorPlurality As String = "errors"

          If sprValidation.Count() = 1 Then
            errorPlurality = "error"
          End If

          ' Now let's see if we actually have a class name, and fall back to the ID if we don't
          If Not String.IsNullOrWhiteSpace(spr.ClassName) Then
            sprValidationMessage = String.Format("The sprite ""{0}"" has the following {1}:", spr.GetEffectiveClassName(), errorPlurality)
          Else
            sprValidationMessage = String.Format("The sprite (ID {0}, no class name provided) has the following {1}:", spr.ID, errorPlurality)
          End If

          ' Append a newline and the contents of the errors
          sprValidationMessage &= Environment.NewLine
          sprValidationMessage &= Validation.GetValidationResultMessage(sprValidation)

          ' Now add that rule to our collection.
          brokenRules.Add(New ValidationResult(sprValidationMessage))
        End If
      Next
    End If

    Return brokenRules

  End Function

End Class
