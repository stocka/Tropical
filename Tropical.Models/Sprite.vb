''' <summary>
''' A single sprite in a <see cref="SpriteSheet">sheet</see>.
''' </summary>
<Serializable()>
Public Class Sprite
  Inherits BaseNotifyPropertyChanged
  Implements IValidatableObject

  Private _id As Guid = Guid.NewGuid()

  ''' <summary>
  ''' Gets or sets the unique identifier.
  ''' </summary>
  ''' <value>
  ''' The unique identifier.
  ''' </value>
  Public Property ID As Guid
    Get
      Return _id
    End Get
    Set(value As Guid)

      If value <> _id Then
        RaisePropertyChanging(Function() ID)
        _id = value
        RaisePropertyChanged(Function() ID)
      End If

    End Set
  End Property

  Private _className As String

  ''' <summary>
  ''' Gets or sets the name of the CSS class
  ''' used for the sprite.
  ''' </summary>
  ''' <value>
  ''' The name of the CSS class used for the sprite.
  ''' </value>
  <Required(ErrorMessage:="The CSS class name is required.")>
  Public Property ClassName As String
    Get
      Return _className
    End Get
    Set(value As String)

      If value <> _filterClassName Then
        RaisePropertyChanging(Function() ClassName)
        _className = value
        RaisePropertyChanged(Function() ClassName)
      End If

    End Set
  End Property

  Private _filterClassName As String

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
    Get
      Return _filterClassName
    End Get
    Set(value As String)

      If value <> _filterClassName Then
        RaisePropertyChanging(Function() FilterClassName)
        _filterClassName = value
        RaisePropertyChanged(Function() FilterClassName)
      End If

    End Set
  End Property

  Private _imagePath As String

  ''' <summary>
  ''' Gets or sets the path to the sprite image.
  ''' </summary>
  ''' <value>
  ''' The path to the sprite image.
  ''' </value>
  Public Property ImagePath As String
    Get
      Return _imagePath
    End Get
    Set(value As String)

      If value <> _imagePath Then
        RaisePropertyChanging(Function() ImagePath)
        _imagePath = value
        RaisePropertyChanged(Function() ImagePath)
      End If

    End Set
  End Property

  Private _hoverImagePath As String

  ''' <summary>
  ''' Gets or sets the path to the sprite image
  ''' that will be displayed on hover.
  ''' </summary>
  ''' <value>
  ''' The path to the sprite image that will
  ''' be displayed on hover.
  ''' </value>
  Public Property HoverImagePath As String
    Get
      Return _hoverImagePath
    End Get
    Set(value As String)

      If value <> _hoverImagePath Then
        RaisePropertyChanging(Function() HoverImagePath)
        _hoverImagePath = value
        RaisePropertyChanged(Function() HoverImagePath)
      End If

    End Set
  End Property

  ''' <summary>
  ''' Gets the &quot;effective&quot; CSS class name
  ''' of this sprite, which will include the <see cref="FilterClassName" />
  ''' as appropriate.
  ''' </summary>
  ''' <returns>The effective CSS class name of the sprite.</returns>
  Public Function GetEffectiveClassName() As String
    Return Sprite.GetEffectiveClassDeclaration(Me.ClassName, Me.FilterClassName, ".")
  End Function

  ''' <summary>
  ''' Given a CSS class name, and optionally a filtering class name, gets
  ''' the equivalent CSS class declaration.
  ''' </summary>
  ''' <param name="className">The name of the CSS class.</param>
  ''' <param name="filterClassName">The name of the filtering CSS class to use.
  ''' This can be null/whitespace.</param>
  ''' <param name="separator">The separator to use between the class
  ''' and filter class names as necessary.</param>
  ''' <returns>The effective CSS class of the sprite.</returns>
  Public Shared Function GetEffectiveClassDeclaration(className As String, filterClassName As String, separator As String) As String

    If String.IsNullOrWhiteSpace(filterClassName) Then
      Return className
    Else
      Return className & separator & filterClassName
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
