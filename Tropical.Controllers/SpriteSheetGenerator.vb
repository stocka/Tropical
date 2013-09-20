Imports System.Drawing

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
  ''' <exception cref="System.ArgumentNullException">
  ''' The <paramref name="sheet" />'s <see cref="SpriteSheet.BaseClassName">base CSS class</see>
  ''' is null or whitespace.
  ''' </exception>
  ''' <exception cref="System.ArgumentNullException">
  ''' The <paramref name="sheet" />'s <see cref="SpriteSheet.BaseFileName">base file name</see>
  ''' is null or whitespace.
  ''' </exception>
  ''' <exception cref="System.ArgumentOutOfRangeException">
  ''' The <see cref="System.Drawing.Size.Width" /> and/or <see cref="System.Drawing.Size.Height" />
  ''' of the <paramref name="sheet"/>'s <see cref="SpriteSheet.ImageDimensions">image dimensions</see>
  ''' is less than or equal to zero.
  ''' </exception>
  Public Sub New(sheet As SpriteSheet)

    If sheet Is Nothing Then
      Throw New ArgumentNullException("sheet")
    End If

    If String.IsNullOrWhiteSpace(sheet.BaseClassName) Then
      Throw New ArgumentNullException("sheet.BaseClassName")
    End If

    If String.IsNullOrWhiteSpace(sheet.BaseFileName) Then
      Throw New ArgumentNullException("sheet.BaseFileName")
    End If

    If sheet.ImageDimensions.Width <= 0 Then
      Throw New ArgumentOutOfRangeException("sheet.ImageDimensions.Width")
    End If

    If sheet.ImageDimensions.Height <= 0 Then
      Throw New ArgumentOutOfRangeException("sheet.ImageDimensions.Height")
    End If

    _sheet = sheet

    ' Place images
    PositionImages()

  End Sub

  Public Function Generate() As String

    Dim spriteSheetBuilder As New System.Text.StringBuilder()

    ' Add the base sprite sheet declaration
    Dim spriteSheetTmplInst As New Tropical.Models.Templates.SpriteSheetTemplate(_sheet)
    spriteSheetBuilder.AppendLine(spriteSheetTmplInst.TransformText())

    ' Now create a template for each specific sprite
    If _sheet.Sprites IsNot Nothing AndAlso _sheet.Sprites.Any() Then

      For Each spr In _sheet.Sprites
        ' Get the equivalent placed sprite, and use that for the template
        Dim spriteTmplInst As New Tropical.Models.Templates.SpriteTemplate(GetPlacedSprite(spr))
        spriteSheetBuilder.AppendLine(spriteTmplInst.TransformText())
      Next

    End If

    ' Return the built CSS
    Return spriteSheetBuilder.ToString()

  End Function

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
      If _imagePositions.ContainsKey(sprite.ImagePath) Then
        .Position = _imagePositions(sprite.ImagePath)
      End If

      If _imagePositions.ContainsKey(sprite.HoverImagePath) Then
        .HoverPosition = _imagePositions(sprite.HoverImagePath)
      End If

    End With

    Return placedSprite

  End Function

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
