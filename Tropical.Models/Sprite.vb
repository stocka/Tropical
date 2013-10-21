Imports System.ComponentModel
Imports System.Collections.ObjectModel

''' <summary>
''' A single sprite in a <see cref="SpriteSheet">sheet</see>.
''' </summary>
<Serializable()>
Public Class Sprite
  Inherits BaseNotifyPropertyChanged
  Implements IValidatableObject
  Implements IDataErrorInfo


  ''' <summary>
  ''' A mapping between the names of all public properties and their
  ''' equivalent property information.
  ''' </summary>
  Private Shared PropertyMappings As ReadOnlyDictionary(Of String, Reflection.PropertyInfo)

  ''' <summary>
  ''' Initializes the <see cref="Sprite"/> class.
  ''' </summary>
  Shared Sub New()

    Dim propertyDict As New Dictionary(Of String, Reflection.PropertyInfo)

    For Each prop In GetType(Sprite).GetProperties()
      propertyDict(prop.Name) = prop
    Next

    Sprite.PropertyMappings =
      New ReadOnlyDictionary(Of String, Reflection.PropertyInfo)(propertyDict)

  End Sub

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
  <Display(Name:="CSS Class",
     ShortName:="Class",
     Description:="This is the name of the CSS class used for the sprite.")>
  <Required(ErrorMessage:="The CSS class name is required.")>
  <RegularExpression(Validation.CssClassNameRegexText, ErrorMessage:="The specified CSS class name is invalid.")>
  Public Property ClassName As String
    Get
      Return _className
    End Get
    Set(value As String)

      If value <> _filterClassName Then
        RaisePropertyChanging(Function() ClassName)
        RaisePropertyChanging(Function() EffectiveClassName)
        _className = value
        RaisePropertyChanged(Function() ClassName)
        RaisePropertyChanged(Function() EffectiveClassName)
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
  <Display(Name:="Filter CSS Class",
    ShortName:="Filter Class",
    Description:="This is the name of the CSS class used to filter the appearance of the sprite.")>
  <RegularExpression(Validation.CssClassNameRegexText, ErrorMessage:="The specified filter CSS class name is invalid.")>
  Public Property FilterClassName As String
    Get
      Return _filterClassName
    End Get
    Set(value As String)

      If value <> _filterClassName Then
        RaisePropertyChanging(Function() FilterClassName)
        RaisePropertyChanging(Function() EffectiveClassName)
        _filterClassName = value
        RaisePropertyChanged(Function() FilterClassName)
        RaisePropertyChanged(Function() EffectiveClassName)
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
  <Display(Name:="Image Path",
     ShortName:="Image Path",
     Description:="This is the path to the image used by the sprite.")>
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
  <Display(Name:="Hover Image Path",
     ShortName:="Hover Image Path",
     Description:="This is the path to the image used by the sprite when hovering.")>
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
  ''' <value>The effective CSS class name of the sprite.</value>
  Public ReadOnly Property EffectiveClassName() As String
    Get
      Return Sprite.GetEffectiveClassDeclaration(Me.ClassName, Me.FilterClassName, ".")
    End Get
  End Property

  ''' <summary>
  ''' Given a CSS class name, and optionally a filtering class name, gets
  ''' the equivalent CSS class declaration.
  ''' </summary>
  ''' <param name="className">The name of the CSS class.</param>
  ''' <param name="filterClassName">The name of the filtering CSS class to use.
  ''' This can be null/whitespace.</param>
  ''' <param name="separator">The separator to use between the class
  ''' and filter class names as necessary.</param>
  ''' <returns>The effective CSS class of the sprite. 
  ''' If no <paramref name="className" /> is provided, this will return
  ''' &quot;(no class specified)&quot;</returns>
  Public Shared Function GetEffectiveClassDeclaration(className As String, filterClassName As String, separator As String) As String

    If String.IsNullOrWhiteSpace(className) Then
      Return "(no class specified)"
    End If

    If String.IsNullOrWhiteSpace(filterClassName) Then
      Return className
    Else
      Return className & separator & filterClassName
    End If

  End Function

#Region "Validation"

  ''' <summary>
  ''' Determines whether the specified object is valid.
  ''' </summary>
  ''' <param name="validationContext">The validation context.</param>
  ''' <returns>
  ''' A collection that holds failed-validation information.
  ''' </returns>
  Public Function Validate(validationContext As ValidationContext) As IEnumerable(Of ValidationResult) Implements IValidatableObject.Validate

    Dim brokenRules As New List(Of ValidationResult)

    ' Validate file paths, if provided
    If Not String.IsNullOrWhiteSpace(Me.ImagePath) AndAlso Not Validation.ValidateFilePath(Me.ImagePath) Then
      brokenRules.Add(New ValidationResult("The specified image path is invalid.", {"ImagePath"}))
    End If

    If Not String.IsNullOrWhiteSpace(Me.HoverImagePath) AndAlso Not Validation.ValidateFilePath(Me.HoverImagePath) Then
      brokenRules.Add(New ValidationResult("The specified hover image path is invalid.", {"HoverImagePath"}))
    End If

    Return brokenRules

  End Function

  ''' <summary>
  ''' Gets an error message indicating what is wrong with this object.
  ''' </summary>
  ''' <returns>An error message indicating what is wrong with this object. 
  ''' The default is an empty string ("").</returns>
  Public ReadOnly Property DataErrorInfoError As String Implements IDataErrorInfo.Error
    Get
      Return Me.DataErrorInfoItem("")
    End Get
  End Property

  ''' <summary>
  ''' Gets the error message for the property with the given name.
  ''' </summary>
  ''' <returns>The error message for the property. 
  ''' The default is an empty string ("").</returns>
  ''' <param name="propertyName">The name of the property whose error message will be
  ''' retrieved.</param>
  Default Public ReadOnly Property DataErrorInfoItem(propertyName As String) As String Implements IDataErrorInfo.Item
    Get

      Dim valResult As Boolean = True
      Dim results As New List(Of ValidationResult)
      Dim context As New ValidationContext(Me)

      If Not String.IsNullOrWhiteSpace(propertyName) Then

        ' Validating a single property.
        context.MemberName = propertyName

        ' Try to get the equivalent value and validate it.
        Dim propInfo As Reflection.PropertyInfo = Nothing

        If Sprite.PropertyMappings.TryGetValue(propertyName, propInfo) Then
          valResult = Validator.TryValidateProperty(propInfo.GetValue(Me), context, results)
        End If

      Else

        ' Validating the entire object.
        valResult = Validator.TryValidateObject(Me, context, results)

      End If

      ' Get the first error message if validation was unsuccessful
      If valResult = False AndAlso results.Any() Then
        Return results.First().ErrorMessage
      Else
        Return String.Empty
      End If

    End Get
  End Property

#End Region

End Class
