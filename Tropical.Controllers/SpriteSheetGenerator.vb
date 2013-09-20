Imports System.ComponentModel.DataAnnotations
Imports System.Drawing
Imports System.IO

''' <summary>
''' Provides methods for generating a sprite sheet.
''' </summary>
Public Class SpriteSheetGenerator

  Private ReadOnly _sheet As SpriteSheet
  Private _imagePositions As Dictionary(Of String, System.Drawing.Point)
  Private _sheetDims As System.Drawing.Size
  Private _sheetRow As Int32 = 0
  Private _sheetColumn As Int32 = 0

  Private Const MaximumImageColumns As Int32 = Int32.MaxValue
  Private Const OverHeightImageWarningFormat As String = "has a height of {0} pixels, which is larger than the sheet image height of {1} pixels"
  Private Const OverWidthImageWarningFormat As String = "has a width of {0} pixels, which is larger than the sheet image width of {1} pixels"

  ''' <summary>
  ''' Gets the current calculated dimensions of the sprite sheet.
  ''' </summary>
  ''' <value>
  ''' The current calculated dimensions.
  ''' </value>
  Public ReadOnly Property Dimensions() As System.Drawing.Size
    Get
      Return _sheetDims
    End Get
  End Property

  ''' <summary>
  ''' Gets the number of sprites in the sheet.
  ''' </summary>
  ''' <value>
  ''' The number of sprites in the sheet.
  ''' </value>
  Public ReadOnly Property SpriteCount() As Int32
    Get
      Return _imagePositions.Count
    End Get
  End Property

  ''' <summary>
  ''' Gets or sets the logger to use.
  ''' </summary>
  ''' <value>
  ''' The logger to use.
  ''' </value>
  Public Property Logger() As ILogger = Nothing

  ''' <summary>
  ''' Initializes a new instance of the <see cref="SpriteSheetGenerator"/> class.
  ''' </summary>
  ''' <param name="sheet">The sheet.</param>
  ''' <exception cref="System.ArgumentNullException">
  ''' The <paramref name="sheet" /> is null.
  ''' </exception>
  ''' <exception cref="ValidationException">
  ''' The provided sprite sheet is invalid. The exception will contain all
  ''' failed-validation information.
  ''' </exception>
  Public Sub New(sheet As SpriteSheet)

    If sheet Is Nothing Then
      Throw New ArgumentNullException("sheet")
    End If

    ' Now validate the sheet
    Dim validationContext As New ValidationContext(sheet)
    Validator.ValidateObject(sheet, validationContext, True)

    _sheet = sheet

    ' Place images
    PositionImages()

  End Sub

  ''' <summary>
  ''' Generates the sprite image sheet and associated CSS stylesheet
  ''' in the specified folder.
  ''' </summary>
  ''' <param name="destinationPath">The destination path.</param>
  ''' <returns><c>true</c> if generation was successful; <c>false</c>
  ''' otherwise. Any messages will be captured in the <see cref="Logger">log</see>.
  ''' If this sheet has no sprites, no action will be performed and this method
  ''' will return <c>false</c>.</returns>
  Public Function Generate(destinationPath As String) As Boolean

    If Me.SpriteCount = 0 Then
      LogError("No sprites have been added to the sheet.")
      Return False
    End If

    ' TODO: better validation around destinationPath

    Dim stylesheetPath As String = Path.Combine(destinationPath, _sheet.BaseFileName & ".css")
    Dim imagePath As String = Path.Combine(destinationPath, _sheet.BaseFileName & ".png")

    ' Write out the stylesheet
    Try
      GenerateStylesheet(stylesheetPath)
    Catch ex As Exception
      LogError("An error was encountered attempting to generate the stylesheet.", exception:=ex)
      Return False
    End Try

    ' Now write out the sprite sheet
    Try
      GenerateSpriteImage(imagePath)
    Catch ex As Exception
      LogError("An error was encountered attempting to generate the sprite sheet.", exception:=ex)
      Return False
    End Try

    ' We're good.
    Return True

  End Function

  ''' <summary>
  ''' Generates and saves the stylesheet for the sprite sheet.
  ''' Will write warnings to the <see cref="Logger">log</see> as appropriate.
  ''' </summary>
  ''' <param name="sheetPath">The path to which the stylesheet will be saved.</param>
  ''' <returns>The stylesheet's contents.</returns>
  Private Function GenerateStylesheet(sheetPath As String) As String

    ' Get the contents.
    Dim sheetContents As String = GenerateStylesheetContents()

    Using stylesheetWriter As New StreamWriter(sheetPath, False, New System.Text.UnicodeEncoding())
      stylesheetWriter.Write(sheetContents)
    End Using

    ' Return the built CSS
    Return sheetContents

  End Function

  ''' <summary>
  ''' Generates the contents of the stylesheet for the sprite sheet.
  ''' Will write warnings to the <see cref="Logger">log</see> as appropriate.
  ''' </summary>
  ''' <returns>The stylesheet's contents.</returns>
  Private Function GenerateStylesheetContents() As String

    Dim encounteredClasses As New HashSet(Of String)
    Dim spriteSheetBuilder As New System.Text.StringBuilder()

    ' Add the base sprite sheet declaration
    Dim spriteSheetTmplInst As New Tropical.Models.Templates.SpriteSheetTemplate(_sheet)
    spriteSheetBuilder.AppendLine(spriteSheetTmplInst.TransformText())

    ' Now create a template for each specific sprite
    If _sheet.Sprites IsNot Nothing AndAlso _sheet.Sprites.Any() Then

      For Each spr In _sheet.Sprites

        ' If we've already seen this class before, throw up a warning.
        If encounteredClasses.Contains(spr.ClassName) Then
          LogWarning("The CSS class """ & spr.ClassName & """ has already been used by a sprite.")
        End If

        ' Get the equivalent placed sprite, and use that for the template
        Dim spriteTmplInst As New Tropical.Models.Templates.SpriteTemplate(GetPlacedSprite(spr))
        spriteSheetBuilder.AppendLine(spriteTmplInst.TransformText())

        ' Add the CSS class to the list of ones we've seen
        encounteredClasses.Add(spr.ClassName)

      Next

    End If

    ' Return the built CSS
    Return spriteSheetBuilder.ToString()

  End Function

  ''' <summary>
  ''' Generates and saves the sprite sheet image.
  ''' Will write warnings to the <see cref="Logger">log</see> as appropriate.
  ''' </summary>
  ''' <param name="imagePath">The path to which the image will be saved.</param>
  Private Sub GenerateSpriteImage(imagePath As String)

    ' If we don't have valid dimensions, don't create a file.
    If _sheetDims.Width = 0 Or _sheetDims.Height = 0 Then
      Return
    End If

    ' Create an image (with ARGB support, as this will be a PNG) with the
    ' calculated dims, as well as the supporting Graphics instance.
    Using sprImage As New Bitmap(_sheetDims.Width, _sheetDims.Height, Imaging.PixelFormat.Format32bppArgb),
       graphics As Graphics = graphics.FromImage(sprImage)

      ' Now iterate over all of our placed images.
      For Each placedImage In _imagePositions
        AddSingleImageToSheet(graphics, placedImage)
      Next

      ' Now save the image as a PNG.
      sprImage.Save(imagePath, Imaging.ImageFormat.Png)

    End Using

  End Sub

#Region "Image Positioning Methods"

  Private Sub PositionImages()

    ' Re-declare core stuff
    _sheetDims = New Drawing.Size()
    _sheetRow = 0
    _sheetColumn = 0
    _imagePositions = New Dictionary(Of String, Drawing.Point)(StringComparer.InvariantCultureIgnoreCase)

    ' Iterate over each sprite
    If _sheet.Sprites IsNot Nothing AndAlso _sheet.Sprites.Any() Then

      For Each spr In _sheet.Sprites

        ' Allocate room for each image
        AllocateImagePosition(spr.ImagePath)
        AllocateImagePosition(spr.HoverImagePath)

      Next

    End If

  End Sub

  ''' <summary>
  ''' Allocates space in the sprite sheet for an image at the specified path.
  ''' If the image path is not defined, or it has already been allocated,
  ''' additional space will not be generated.
  ''' </summary>
  ''' <param name="imagePath">The path to the image.</param>
  Private Sub AllocateImagePosition(imagePath As String)

    ' Make sure the path is defined and we haven't already mapped a path to it
    If Not String.IsNullOrWhiteSpace(imagePath) AndAlso Not _imagePositions.ContainsKey(imagePath) Then

      ' Okay, we haven't already mapped a path to this yet.
      ' Allocate a point for this image.
      Dim imageLocation As New Drawing.Point()
      With imageLocation
        .X = _sheetColumn * _sheet.ImageDimensions.Width
        .Y = _sheetRow * _sheet.ImageDimensions.Height
      End With

      ' Add it to the dictionary, keyed to the specified file.
      _imagePositions.Add(imagePath, imageLocation)

      ' Recalculate the dimensions of the overall sheet.
      _sheetDims.Width = Math.Max(_sheetDims.Width, imageLocation.X + _sheet.ImageDimensions.Width)
      _sheetDims.Height = Math.Max(_sheetDims.Height, imageLocation.Y + _sheet.ImageDimensions.Height)

      ' Let's see at this point if we're supposed to start on a new row.
      If _sheetColumn = MaximumImageColumns - 1 Then
        ' Okay, start a new row.
        _sheetRow += 1
        _sheetColumn = 0
      Else
        ' No, just increment the column index.
        _sheetColumn += 1
      End If

    End If

  End Sub

  ''' <summary>
  ''' Gets the equivalent sprite placement for the provided sprite.
  ''' </summary>
  ''' <param name="sprite">The sprite.</param>
  ''' <returns>The equivalent sprite placement.</returns>
  Private Function GetPlacedSprite(sprite As Sprite) As PlacedSprite

    Dim placedSprite As New PlacedSprite()
    With placedSprite

      .BaseClassName = _sheet.BaseClassName
      .ClassName = sprite.ClassName

      ' Add sprite positions for each style if we have them
      If Not String.IsNullOrWhiteSpace(sprite.ImagePath) AndAlso _imagePositions.ContainsKey(sprite.ImagePath) Then
        .Position = _imagePositions(sprite.ImagePath)
      End If

      If Not String.IsNullOrWhiteSpace(sprite.HoverImagePath) AndAlso _imagePositions.ContainsKey(sprite.HoverImagePath) Then
        .HoverPosition = _imagePositions(sprite.HoverImagePath)
      End If

    End With

    Return placedSprite

  End Function

#End Region

#Region "Image Generation Methods"

  ''' <summary>
  ''' Adds a single image to the sprite sheet (as encapsulated by <paramref name="graphics" />).
  ''' </summary>
  ''' <param name="graphics">The graphics object for the sprite sheet.</param>
  ''' <param name="imageInfo">The image to add.</param>
  Private Sub AddSingleImageToSheet(graphics As Graphics, imageInfo As KeyValuePair(Of String, Point))

    Dim imagePath As String = imageInfo.Key
    Dim imageFileInfo As New FileInfo(imagePath)

    ' Log a warning if the file doesn't exist.
    If Not imageFileInfo.Exists() Then
      LogImageWarning(imagePath, "does not exist")
      Return
    End If

    Try

      Using spriteImage As Image = Image.FromFile(imagePath)

        ' Do some simple bounds checking, and log a warning if it's weird.
        If spriteImage.Height > _sheet.ImageDimensions.Height Then
          LogImageWarning(imagePath, String.Format(OverHeightImageWarningFormat, spriteImage.Height, _sheet.ImageDimensions.Height))
        End If

        If spriteImage.Width > _sheet.ImageDimensions.Width Then
          LogImageWarning(imagePath, String.Format(OverWidthImageWarningFormat, spriteImage.Width, _sheet.ImageDimensions.Width))
        End If

        ' Draw the image at the specified point
        graphics.DrawImage(spriteImage, imageInfo.Value)

      End Using

    Catch ex As Exception
      LogImageWarning(imagePath, "could not be loaded", exception:=ex)
    End Try

  End Sub

#End Region

#Region "Logging Methods"

  ''' <summary>
  ''' Logs a debug message.
  ''' </summary>
  ''' <param name="message">The message text.</param>
  ''' <param name="exception">The associated exception. Optional.</param>
  Private Sub LogDebug(message As String, Optional exception As Exception = Nothing)

    If Me.Logger IsNot Nothing Then
      Me.Logger.Debug(message, exception:=exception)
    End If

  End Sub

  ''' <summary>
  ''' Logs an informational message.
  ''' </summary>
  ''' <param name="message">The message text.</param>
  ''' <param name="exception">The associated exception. Optional.</param>
  Private Sub LogInfo(message As String, Optional exception As Exception = Nothing)

    If Me.Logger IsNot Nothing Then
      Me.Logger.Information(message, exception:=exception)
    End If

  End Sub

  ''' <summary>
  ''' Logs a warning message for the specified image.
  ''' </summary>
  ''' <param name="imagePath">The path to the image.</param>
  ''' <param name="message">The warning to include.</param>
  ''' <param name="exception">The associated exception. Optional.</param>
  Private Sub LogImageWarning(imagePath As String, message As String, Optional exception As Exception = Nothing)
    LogWarning("The image """ & imagePath & """ " & message & ".", exception:=exception)
  End Sub

  ''' <summary>
  ''' Logs a warning message.
  ''' </summary>
  ''' <param name="message">The message text.</param>
  ''' <param name="exception">The associated exception. Optional.</param>
  Private Sub LogWarning(message As String, Optional exception As Exception = Nothing)

    If Me.Logger IsNot Nothing Then
      Me.Logger.Warning(message, exception:=exception)
    End If

  End Sub

  ''' <summary>
  ''' Logs an error message.
  ''' </summary>
  ''' <param name="message">The message text.</param>
  ''' <param name="exception">The associated exception. Optional.</param>
  Private Sub LogError(message As String, Optional exception As Exception = Nothing)

    If Me.Logger IsNot Nothing Then
      Me.Logger.Error(message, exception:=exception)
    End If

  End Sub

#End Region

End Class
