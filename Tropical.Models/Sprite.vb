<Serializable()>
Public Class Sprite

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
  Public Property ClassName As String

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

End Class
