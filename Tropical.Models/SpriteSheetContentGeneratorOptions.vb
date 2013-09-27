''' <summary>
''' Contains options for automatically generating the contents
''' of a sprite sheet from a directory.
''' </summary>
Public Class SpriteSheetContentGeneratorOptions
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
  Public Property BaseClassName() As String

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
  Public Property BaseFileName() As String

  ''' <summary>
  ''' Gets or sets the set of file extensions from which
  ''' images will be generated.
  ''' </summary>
  ''' <value>
  ''' The set of file extensions from which images will be generated.
  ''' </value>
  Public Property FileExtensions() As String() = {"jpg", "jpeg", "gif", "png"}

  ''' <summary>
  ''' Gets or sets the search pattern that will be used
  ''' to retrieve files in the directory.
  ''' </summary>
  ''' <value>
  ''' The search pattern that will be used to retrieve
  ''' files in the directory.
  ''' </value>
  Public Property FileSearchPattern() As String = "*"

  ''' <summary>
  ''' Gets or sets the delimiters that will be used to
  ''' separate out CSS class names.
  ''' </summary>
  ''' <value>
  ''' The delimiters that will be used to separate out CSS class
  ''' names.
  ''' </value>
  Public Property ClassDelimiters() As Char() = {"_"c, "-"c, " "c}

  ''' <summary>
  ''' The string that will be used to join any leftover classes
  ''' after filter and/or hover classes have been removed.
  ''' </summary>
  Public Const ClassJoinString As String = "-"

  ''' <summary>
  ''' Gets or sets the collection of CSS class names
  ''' that will be interpreted as hover classes.
  ''' </summary>
  ''' <value>
  ''' The collection of CSS class names that will be
  ''' interpreted as hover classes.
  ''' </value>
  Public Property HoverClassNames() As String() = {}

  ''' <summary>
  ''' Gets or sets the collection of CSS class names
  ''' that will be interpreted as filter classes.
  ''' </summary>
  ''' <value>
  ''' The collection of CSS class names that will be
  ''' interpreted as filter classes.
  ''' </value>
  Public Property FilterClassNames() As String() = {}

  ''' <summary>
  ''' Gets or sets the height, in pixels, of each image.
  ''' </summary>
  ''' <value>
  ''' The height, in pixels, of each image.
  ''' </value>
  <Range(1, Int32.MaxValue, ErrorMessage:="Image height must be a positive value.")>
  Public Property ImageHeight() As Int32 = 16

  ''' <summary>
  ''' Gets or sets the width, in pixels, of each image.
  ''' </summary>
  ''' <value>
  ''' The width, in pixels, of each image.
  ''' </value>
  <Range(1, Int32.MaxValue, ErrorMessage:="Image width must be a positive value.")>
  Public Property ImageWidth() As Int32 = 16

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
    If Me.ImageHeight <= 0 Then
      brokenRules.Add(New ValidationResult("The specified image height is invalid.", {"ImageHeight"}))
    End If

    If Me.ImageWidth <= 0 Then
      brokenRules.Add(New ValidationResult("The specified image width is invalid.", {"ImageWidth"}))
    End If

    ' Validate filter class names if provided
    ValidateClassSet(brokenRules, Me.FilterClassNames, "filter class names", "filter", "FilterClassNames")

    ' Validate hover class names if provided
    ValidateClassSet(brokenRules, Me.HoverClassNames, "hover class names", "hover", "HoverClassNames")

    ' Validate class delimiter contents
    If Me.ClassDelimiters Is Nothing Then
      brokenRules.Add(New ValidationResult("The set of class delimiters cannot be null.", {"ClassDelimiters"}))
    ElseIf Not Me.ClassDelimiters.Any() AndAlso (Me.FilterClassNames.Any() Or Me.HoverClassNames.Any()) Then
      ' We don't have any delimiters, so there's no way to identify filter or hover classes.
      brokenRules.Add(New ValidationResult("If hover and/or filter classes are specified, class delimiters must also be specified.", {"ClassDelimiters"}))
    End If

    ' Now validate that there's no overlap between filter class names and hover class names.
    If Me.FilterClassNames IsNot Nothing AndAlso Me.FilterClassNames.Any() AndAlso
      Me.HoverClassNames IsNot Nothing AndAlso Me.HoverClassNames.Any() Then

      ' Use a case-insensitive comparison, because we're going to be 
      ' getting these from file names, which will be case-insensitive.
      For Each duplicateClass In Me.FilterClassNames.Intersect(Me.HoverClassNames, StringComparer.InvariantCultureIgnoreCase)
        brokenRules.Add(New ValidationResult("The CSS class """ & duplicateClass & """ is specified as both a filter and hover class.", {"FilterClassNames", "HoverClassNames"}))
      Next

    End If

    Return brokenRules

  End Function

  ''' <summary>
  ''' Validates a set of CSS classes.
  ''' </summary>
  ''' <param name="brokenRules">The broken rules collection to add
  ''' failed-validation information.</param>
  ''' <param name="classSet">The set of CSS classes.</param>
  ''' <param name="friendlyCollectionName">The user-friendly name for the set.</param>
  ''' <param name="friendlyClassName">The user-friendly name for a class.</param>
  ''' <param name="memberName">The name of the equivalent member.</param>
  Private Shared Sub ValidateClassSet(brokenRules As List(Of ValidationResult),
                                      classSet As IEnumerable(Of String),
                                      friendlyCollectionName As String,
                                      friendlyClassName As String,
                                      memberName As String)

    Dim warnedNullWhitespace As Boolean = False

    If classSet Is Nothing Then
      brokenRules.Add(New ValidationResult("The set of " & friendlyCollectionName & " cannot be null.", {memberName}))
    Else

      For Each cssClass As String In classSet

        ' First validate that it's not null/whitespace, then
        ' actually validate the class.
        If String.IsNullOrWhiteSpace(cssClass) Then
          ' Only show this error once per-collection.
          If Not warnedNullWhitespace Then
            brokenRules.Add(New ValidationResult("The set of " & friendlyCollectionName & " cannot contain null/whitespace classes.", {memberName}))
            warnedNullWhitespace = True
          End If
        ElseIf Not Validation.ValidateCssClass(cssClass) Then
          brokenRules.Add(New ValidationResult("The CSS class """ & cssClass & """ is not a valid " & friendlyClassName & " class.", {memberName}))
        End If

      Next
    End If

  End Sub

End Class
