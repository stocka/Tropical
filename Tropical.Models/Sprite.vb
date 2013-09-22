''' <summary>
''' A single sprite in a <see cref="SpriteSheet">sheet</see>.
''' </summary>
<Serializable()>
Public Class Sprite
  Implements IValidatableObject

  ''' <summary>
  ''' Gets or sets the unique identifier.
  ''' </summary>
  ''' <value>
  ''' The unique identifier.
  ''' </value>
  Public Property ID As New Guid

  ''' <summary>
  ''' Gets or sets the name of the CSS class
  ''' used for the sprite.
  ''' </summary>
  ''' <value>
  ''' The name of the CSS class used for the sprite.
  ''' </value>
  <Required(ErrorMessage:="The CSS class name is required.")>
  Public Property ClassName As String

  ''' <summary>
  ''' Gets or sets the name of the CSS class used to filter
  ''' the appearance of the sprite.  For example, this could
  ''' be a class such as &quot;disabled&quot;, &quot;accent&quot;,
  ''' or &quot;muted&quot;
  ''' </summary>
  ''' <value>
  ''' The name of the CSS class used to filter the appearance of the
  ''' sprite.
  ''' </value>
  Public Property FilterClassName As String

  ''' <summary>
  ''' Gets or sets the path to the sprite image.
  ''' </summary>
  ''' <value>
  ''' The path to the sprite image.
  ''' </value>
  Public Property ImagePath As String

  ''' <summary>
  ''' Gets or sets the path to the sprite image
  ''' that will be displayed on hover.
  ''' </summary>
  ''' <value>
  ''' The path to the sprite image that will
  ''' be displayed on hover.
  ''' </value>
  Public Property HoverImagePath As String

  ''' <summary>
  ''' Gets the &quot;effective&quot; CSS class name
  ''' of this sprite, which will include the <see cref="FilterClassName" />
  ''' as appropriate.
  ''' </summary>
  ''' <returns>The effective CSS class name of the sprite.</returns>
  Public Function GetEffectiveClassName() As String

    ' See if we have a filter class specified
    If String.IsNullOrWhiteSpace(Me.FilterClassName) Then
      Return Me.ClassName
    Else
      Return Me.ClassName & "." & Me.FilterClassName
    End If

  End Function

  ''' <summary>
  ''' Determines whether the specified object is valid.
  ''' </summary>
  ''' <param name="validationContext">The validation context.</param>
  ''' <returns>
  ''' A collection that holds failed-validation information.
  ''' </returns>
  Public Function Validate(validationContext As ValidationContext) As IEnumerable(Of ValidationResult) Implements IValidatableObject.Validate

    Dim brokenRules As New List(Of ValidationResult)

    ' Validate the class name
    If Not String.IsNullOrWhiteSpace(Me.ClassName) AndAlso Not Validation.ValidateCssClass(Me.ClassName) Then
      brokenRules.Add(New ValidationResult("The specified CSS class name is invalid.", {"ClassName"}))
    End If

    ' Validate the filter class name, if provided
    If Not String.IsNullOrWhiteSpace(Me.FilterClassName) AndAlso Not Validation.ValidateCssClass(Me.FilterClassName) Then
      brokenRules.Add(New ValidationResult("The specified filter CSS class name is invalid.", {"FilterClassName"}))
    End If

    ' Validate file paths, if provided
    If Not String.IsNullOrWhiteSpace(Me.ImagePath) AndAlso Not Validation.ValidateFilePath(Me.ImagePath) Then
      brokenRules.Add(New ValidationResult("The specified image path is invalid.", {"ImagePath"}))
    End If

    If Not String.IsNullOrWhiteSpace(Me.HoverImagePath) AndAlso Not Validation.ValidateFilePath(Me.HoverImagePath) Then
      brokenRules.Add(New ValidationResult("The specified hover image path is invalid.", {"HoverImagePath"}))
    End If

    Return brokenRules

  End Function

End Class
