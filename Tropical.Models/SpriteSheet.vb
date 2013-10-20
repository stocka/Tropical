''' <summary>
''' A sprite sheet.
''' </summary>
<Serializable()>
Public Class SpriteSheet
  Inherits BaseNotifyPropertyChanged
  Implements IValidatableObject

  Private _baseClassName As String

  ''' <summary>
  ''' Gets or sets the name of the CSS class
  ''' that will be used for all sprites.
  ''' </summary>
  ''' <value>
  ''' The name of the CSS class that will
  ''' be used for all sprites.
  ''' </value>
  <Display(Name:="Base CSS Class",
     ShortName:="CSS Class",
     Description:="This is the CSS class that will be used for all sprites.")>
  <Required(ErrorMessage:="The base CSS class name is required.")>
  <RegularExpression(Validation.CssClassNameRegexText, ErrorMessage:="The specified base CSS class name is invalid.")>
  Public Property BaseClassName As String
    Get
      Return _baseClassName
    End Get
    Set(value As String)

      If value <> _baseClassName Then
        RaisePropertyChanging(Function() BaseClassName)
        _baseClassName = value
        RaisePropertyChanged(Function() BaseClassName)
      End If

    End Set
  End Property

  Private _baseFileName As String

  ''' <summary>
  ''' Gets or sets the base filename that will
  ''' be used to generate the sprite sheet and associated
  ''' CSS stylesheet.
  ''' </summary>
  ''' <value>
  ''' The base filename that will be used to generate
  ''' the sprite sheet and associated CSS stylesheet.
  ''' </value>
  <Display(Name:="Base File Name",
     ShortName:="Base File Name",
     Description:="This is the base file name that will be used to generate the sprite sheet and associated CSS stylesheet.")>
  <Required(ErrorMessage:="The base file name is required.")>
  Public Property BaseFileName As String
    Get
      Return _baseFileName
    End Get
    Set(value As String)

      If value <> _baseFileName Then
        RaisePropertyChanging(Function() BaseFileName)
        _baseFileName = value
        RaisePropertyChanged(Function() BaseFileName)
      End If

    End Set
  End Property

  Private _imageHeight As Int32 = 16

  ''' <summary>
  ''' Gets or sets the height, in pixels, of each image.
  ''' </summary>
  ''' <value>
  ''' The height, in pixels, of each image.
  ''' </value>
  <Display(Name:="Image Height",
     ShortName:="Image Height",
     Description:="This is the height, in pixels, of each image.")>
  <Range(1, Int32.MaxValue, ErrorMessage:="Image height must be a positive value.")>
  Public Property ImageHeight() As Int32
    Get
      Return _imageHeight
    End Get
    Set(value As Int32)

      If value <> _imageHeight Then
        RaisePropertyChanging(Function() ImageHeight)
        _imageHeight = value
        RaisePropertyChanged(Function() ImageHeight)
      End If

    End Set
  End Property

  Private _imageWidth As Int32 = 16

  ''' <summary>
  ''' Gets or sets the width, in pixels, of each image.
  ''' </summary>
  ''' <value>
  ''' The width, in pixels, of each image.
  ''' </value>
  <Display(Name:="Image Width",
     ShortName:="Image Width",
     Description:="This is the width, in pixels, of each image.")>
  <Range(1, Int32.MaxValue, ErrorMessage:="Image width must be a positive value.")>
  Public Property ImageWidth() As Int32
    Get
      Return _imageWidth
    End Get
    Set(value As Int32)

      If value <> _imageWidth Then
        RaisePropertyChanging(Function() ImageWidth)
        _imageWidth = value
        RaisePropertyChanged(Function() ImageWidth)
      End If

    End Set
  End Property

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

    ' Validate the file name
    If Not String.IsNullOrWhiteSpace(Me.BaseFileName) AndAlso Not Validation.ValidateFileName(Me.BaseFileName) Then
      brokenRules.Add(New ValidationResult("The specified base file name is invalid.", {"BaseFileName"}))
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
            sprValidationMessage = String.Format("The sprite ""{0}"" has the following {1}:", spr.EffectiveClassName(), errorPlurality)
          Else
            sprValidationMessage = String.Format("The sprite (ID {0}, no class name provided) has the following {1}:", spr.ID, errorPlurality)
          End If

          ' Append a newline and the contents of the errors
          sprValidationMessage &= Environment.NewLine
          sprValidationMessage &= Validation.GetValidationResultMessage(sprValidation, False)

          ' Now add that rule to our collection.
          brokenRules.Add(New ValidationResult(sprValidationMessage))
        End If
      Next
    End If

    Return brokenRules

  End Function

End Class
