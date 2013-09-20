''' <summary>
''' A sprite sheet.
''' </summary>
<Serializable()>
Public Class SpriteSheet

  ''' <summary>
  ''' Gets or sets the name of the CSS class
  ''' that will be used for all sprites.
  ''' </summary>
  ''' <value>
  ''' The name of the CSS class that will
  ''' be used for all sprites.
  ''' </value>
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

End Class
