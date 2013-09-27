Imports System.ComponentModel.DataAnnotations
Imports System.IO

''' <summary>
''' Contains functionality for automatically generating a
''' sprite sheet from the contents of a given folder.
''' </summary>
Public Class SpriteSheetContentGenerator

  Private Shared ReadOnly DigitRegex As New System.Text.RegularExpressions.Regex("^\d+$", Text.RegularExpressions.RegexOptions.IgnoreCase)
  Private ReadOnly _options As SpriteSheetContentGeneratorOptions = Nothing
  Private _logger As ILogger = New BlackholeLogger()
  Private _encounteredSprites As Dictionary(Of String, Sprite)

  ''' <summary>
  ''' Gets or sets the logger to use.
  ''' </summary>
  ''' <value>
  ''' The logger to use.
  ''' </value>
  ''' <exception cref="ArgumentNullException">
  ''' The <see cref="ILogger" /> instance specified by <paramref name="value" />
  ''' is <c>null</c>.
  ''' </exception>
  Public Property Logger() As ILogger
    Get
      Return _logger
    End Get
    Set(value As ILogger)

      If value Is Nothing Then
        Throw New ArgumentNullException("value")
      End If

      Me._logger = value

    End Set
  End Property

  ''' <summary>
  ''' Initializes a new instance of the <see cref="SpriteSheetContentGenerator"/> class.
  ''' </summary>
  ''' <param name="options">The content generation options to use.</param>
  ''' <exception cref="System.ArgumentNullException">
  ''' The <paramref name="options" /> are null.
  ''' </exception>
  ''' <exception cref="ValidationException">
  ''' The provided options are invalid. The exception will contain all
  ''' failed-validation information.
  ''' </exception>
  Public Sub New(options As SpriteSheetContentGeneratorOptions)

    If options Is Nothing Then
      Throw New ArgumentNullException("options")
    End If

    ' Now validate the options
    Dim validationContext As New ValidationContext(options)
    Validator.ValidateObject(options, validationContext, True)

    Me._options = options

  End Sub

  ''' <summary>
  ''' Generates a sprite sheet instance from the contents
  ''' of the specified directory.
  ''' </summary>
  ''' <param name="sourcePath">The path to the content directory.</param>
  ''' <returns>The generated sprite sheet, or <c>null</c>
  ''' if the sprite sheet could not be generated.
  ''' Any messages will be captured in the <see cref="Logger">log</see>.
  ''' </returns>
  ''' <exception cref="System.ArgumentNullException">
  ''' The specified <paramref name="sourcePath" /> is <c>null</c> or whitespace.
  ''' </exception>
  ''' <exception cref="System.ArgumentException">
  ''' The specified <paramref name="sourcePath" /> is not a valid path.
  ''' </exception>
  ''' <exception cref="System.IO.DirectoryNotFoundException">
  ''' The directory specified by <paramref name="sourcePath" /> does not
  ''' exist or is a file.
  ''' </exception>
  Public Function Generate(sourcePath As String) As SpriteSheet

    ' Validate the directory
    If String.IsNullOrWhiteSpace(sourcePath) Then
      Throw New ArgumentNullException("sourcePath")
    End If

    If Not Validation.ValidateDirectoryPath(sourcePath) Then
      Throw New ArgumentException("The directory specified by sourcePath is not a valid path.", "sourcePath")
    End If

    If Not Directory.Exists(sourcePath) Then
      Throw New DirectoryNotFoundException()
    End If

    ' Condition our file extensions to at least be non-null
    If Me._options.FileExtensions Is Nothing Then
      Me._options.FileExtensions = {}
    End If

    ' Set up our internal state.
    _encounteredSprites = New Dictionary(Of String, Sprite)

    Try

      ' Create a new sprite sheet and copy over options
      Dim sheet As New SpriteSheet()

      With sheet
        .BaseClassName = Me._options.BaseClassName
        .BaseFileName = Me._options.BaseFileName
        .ImageDimensions = New System.Drawing.Size(Me._options.ImageWidth, Me._options.ImageHeight)
      End With

      ' Process each file in the directory.
      For Each filePath In Directory.EnumerateFiles(sourcePath, Me._options.FileSearchPattern)
        ' Make sure we should process this.
        If Not ShouldProcessFile(filePath) Then
          Continue For
        End If

        ' Let's do it.
        ProcessFile(filePath)
      Next

      ' Now add each encountered sprite to the sheet.
      For Each encounteredSprite In _encounteredSprites.Values
        sheet.Sprites.Add(encounteredSprite)
      Next

      Logger.Information(_encounteredSprites.Count & " sprite(s) created.")

      Return sheet

    Catch ex As Exception
      Logger.Error("An error was encountered attempting to generate the sprite sheet.", exception:=ex)
      Return Nothing
    End Try

  End Function

  ''' <summary>
  ''' Processes a file by creating/updating a sprite entry as appropriate.
  ''' </summary>
  ''' <param name="filePath">The file path.</param>
  Private Sub ProcessFile(filePath As String)

    Dim classes As String() = Nothing
    Dim filterClass As String = Nothing
    Dim hoverClass As String = Nothing

    ' Indicate that we're looking
    Logger.Debug("Processing file """ & filePath & """.")

    ' Make sure the file name was parsed correctly.
    If ParseFileName(filePath, classes, filterClass, hoverClass) Then

      ' Figure out the effective class name.
      Dim effectiveClassName As String = GetEffectiveClassName(classes, filterClass)

      ' See if we've already encountered this sprite.
      If _encounteredSprites.ContainsKey(effectiveClassName) Then

        Dim existingSprite As Sprite = _encounteredSprites(effectiveClassName)
        Logger.Debug("Updating sprite """ & effectiveClassName & """ from file """ & filePath & """.")

        ' Try to set the hover image path if a hover class was found,
        ' otherwise set the regular image path.
        If Not String.IsNullOrWhiteSpace(hoverClass) Then
          TryDefineImagePath(effectiveClassName, existingSprite.HoverImagePath, filePath, "hover image")
        Else
          TryDefineImagePath(effectiveClassName, existingSprite.ImagePath, filePath, "image")
        End If

      Else

        ' This is a new sprite. Let's set that up.
        Dim spr As New Sprite()
        With spr

          ' This is subtly different from our previous call, but this time
          ' we're NOT including any filtering class.
          .ClassName = GetEffectiveClassName(classes, Nothing)

          ' This is why - we're using the filter class here.
          .FilterClassName = filterClass

          ' If we have a hover class defined, this is our hover image.
          ' Otherwise, it's our regular image.
          If Not String.IsNullOrWhiteSpace(hoverClass) Then
            .HoverImagePath = filePath
          Else
            .ImagePath = filePath
          End If

        End With

        ' Tell the debug log we're adding it.
        Logger.Debug("Adding sprite """ & effectiveClassName & """ from file """ & filePath & """.")

        ' Add it to the collection and the sheet
        _encounteredSprites.Add(effectiveClassName, spr)

      End If

    End If

  End Sub

  ''' <summary>
  ''' Attempts to set the value of <paramref name="updatePathValue" />
  ''' based on the value of <paramref name="newPathValue" />, but will
  ''' instead log a warning if <paramref name="updatePathValue" />
  ''' has already been defined.
  ''' </summary>
  ''' <param name="className">The name of the affected CSS class.</param>
  ''' <param name="updatePathValue">A reference to the image path value to update.</param>
  ''' <param name="newPathValue">The new image path value to use.</param>
  ''' <param name="friendlyName">The user-friendly name of
  ''' the image path being defined.</param>
  Private Sub TryDefineImagePath(className As String,
                                 ByRef updatePathValue As String,
                                 newPathValue As String,
                                 friendlyName As String)

    ' If it's already defined, throw up a warning
    If Not String.IsNullOrWhiteSpace(updatePathValue) AndAlso
      Not updatePathValue.Equals(newPathValue, StringComparison.InvariantCultureIgnoreCase) Then

      Dim warningMessage As String =
        String.Format("The CSS class ""{0}"" has a {1} of ""{2}"" already defined, but another {1} of ""{3}"" was also specified.",
                      className, friendlyName, updatePathValue, newPathValue)
      Logger.Warning(warningMessage)

    Else
      updatePathValue = newPathValue
    End If

  End Sub

  ''' <summary>
  ''' Parses the name of a file into its constituent CSS class elements.
  ''' </summary>
  ''' <param name="filePath">The file path.</param>
  ''' <param name="classes">The array of standard CSS classes
  ''' that were parsed. This parameter is passed uninitialized.</param>
  ''' <param name="filterClass">The filter class, if any,
  ''' that was parsed. This parameter is passed uninitialized.</param>
  ''' <param name="hoverClass">The hover class, if any,
  ''' that was parsed. This parameter is passed uninitialized.</param>
  ''' <returns><c>true</c> if the file name was parsed successfully,
  ''' <c>false</c> otherwise.</returns>
  Private Function ParseFileName(filePath As String,
                                 ByRef classes As String(),
                                 ByRef filterClass As String,
                                 ByRef hoverClass As String) As Boolean

    ' Get just the filename without the extension
    Dim fileName As String = Path.GetFileNameWithoutExtension(filePath)

    ' Split the file name up into different classes
    ' based on its delimiters.
    Dim parsedClasses As String() = fileName.Split(Me._options.ClassDelimiters, StringSplitOptions.RemoveEmptyEntries)

    ' Make sure we found things.
    If Not parsedClasses.Any() Then
      Logger.Warning("No class names could be determined from the file """ & filePath & """.")
      Return False
    End If

    ' Debug our list of classes
    Logger.Debug("Determined classes of """ & String.Join(" ", parsedClasses) & """.")

    Dim regularClasses As New List(Of String)

    ' Now iterate through everything we found.
    For Each parsedClass In parsedClasses

      ' Trim whitespace.
      parsedClass = parsedClass.Trim()

      ' Make sure we still have something.
      If String.IsNullOrWhiteSpace(parsedClass) Then
        Continue For
      End If

      ' Skip over classes that are just numbers.
      If DigitRegex.IsMatch(parsedClass) Then
        Logger.Debug("Skipping parsed class of """ & parsedClass & """ as it consists entirely of decimal digits.")
        Continue For
      End If

      ' See if it's one of our special options.
      If Me._options.FilterClassNames.Contains(parsedClass, StringComparer.InvariantCultureIgnoreCase) Then
        ' Filter class. Attempt to update it if it doesn't already exist.
        TryDefineClass(filePath, filterClass, parsedClass, "filter")
      ElseIf Me._options.HoverClassNames.Contains(parsedClass, StringComparer.InvariantCultureIgnoreCase) Then
        ' Hover class. Attempt to update it if it doesn't already exist.
        TryDefineClass(filePath, hoverClass, parsedClass, "hover")
      Else
        ' Nothing special.
        ' We still need to make sure it's a valid class name, though.
        If Not Validation.ValidateCssClass(parsedClass) Then
          Logger.Warning("The CSS class """ & parsedClass & """ was retrieved from the file name """ & fileName & """, but it was ignored because is not a valid CSS class.")
        Else
          ' Add it to our collection of regular classes.
          regularClasses.Add(parsedClass)
        End If
      End If
    Next

    ' After all of that, return true if we have any regular classes.
    If regularClasses.Any() Then
      ' Copy over our regular class array.
      classes = regularClasses.ToArray()
      Return True
    Else
      Logger.Warning("The file name """ & filePath & """ consists only of valid hover and/or filter classes.")
      Return False
    End If

  End Function

  ''' <summary>
  ''' Gets the effective CSS class name based on the collection
  ''' of regular classes, optionally including a filtering class
  ''' if it has been specified.
  ''' </summary>
  ''' <param name="regularClasses">The regular classes to use.
  ''' It is assumed that this contains values and that each value
  ''' has already been validated.</param>
  ''' <param name="filterClass">The filtering class to use.
  ''' This can be null/whitespace.</param>
  ''' <returns>The effective CSS class name given the classes
  ''' that have been provided to this method.</returns>
  Private Function GetEffectiveClassName(regularClasses As String(),
                                         filterClass As String) As String

    ' Join the regular classes by our configured delimiter
    Dim className As String = String.Join(SpriteSheetContentGeneratorOptions.ClassJoinString, regularClasses)

    ' Now get our effective declaration.
    Return Sprite.GetEffectiveClassDeclaration(className, filterClass, ".")

  End Function

  ''' <summary>
  ''' Attempts to set the value of <paramref name="updateClassValue" />
  ''' based on the value of <paramref name="newClassValue" />, but will
  ''' instead log a warning if <paramref name="updateClassValue" />
  ''' has already been defined.
  ''' </summary>
  ''' <param name="filePath">The path to the originating file.</param>
  ''' <param name="updateClassValue">A reference to the CSS
  ''' class value to update.</param>
  ''' <param name="newClassValue">The new class value to use.</param>
  ''' <param name="friendlyName">The user-friendly name of
  ''' the CSS class.</param>
  Private Sub TryDefineClass(filePath As String,
                             ByRef updateClassValue As String,
                             newClassValue As String,
                             friendlyName As String)

    ' If it's already defined, throw up a warning
    If Not String.IsNullOrWhiteSpace(updateClassValue) AndAlso
      Not updateClassValue.Equals(newClassValue, StringComparison.InvariantCultureIgnoreCase) Then

      Dim warningMessage As String =
        String.Format("The file name ""{0}"" has a {1} class of ""{2}"" already defined, but another {1} class of ""{3}"" was also specified.",
                      filePath, friendlyName, updateClassValue, newClassValue)
      Logger.Warning(warningMessage)

    Else
      updateClassValue = newClassValue
    End If

  End Sub

  ''' <summary>
  ''' Determines if the file specified by <paramref name="filePath" />
  ''' should be included in the sprite sheet.
  ''' </summary>
  ''' <param name="filePath">The file path.</param>
  ''' <returns><c>true</c> if the file should be included in the
  ''' sprite sheet, <c>false</c> otherwise.</returns>
  Private Function ShouldProcessFile(filePath As String) As Boolean

    ' Make sure we have file extensions to filter on
    If Not Me._options.FileExtensions.Any() Then
      Return True
    Else

      Dim fileExt As String = Path.GetExtension(filePath)

      ' Filter out the leading . and look for it
      If fileExt.Length > 1 Then
        fileExt = fileExt.Substring(1)
        Return Me._options.FileExtensions.Contains(fileExt, StringComparer.InvariantCultureIgnoreCase)
      Else
        Return False
      End If

    End If

  End Function

End Class
