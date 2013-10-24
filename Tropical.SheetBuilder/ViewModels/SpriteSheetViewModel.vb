Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.Collections.ObjectModel
Imports Tropical.Models

''' <summary>
''' A view model for interacting with a sprite sheet.
''' </summary>
Public Class SpriteSheetViewModel
  Inherits BaseNotifyPropertyChanged

  ''' <summary>
  ''' A dictionary for all descriptions for fields on a sprite sheet.
  ''' The key is the field name and the value is the equivalent description,
  ''' as determined by the <see cref="DisplayAttribute.Description" />
  ''' that has been specified for the field.
  ''' </summary>
  Shared ReadOnly SheetFieldDescriptions As ReadOnlyDictionary(Of String, String)

  ''' <summary>
  ''' A dictionary for all descriptions for fields on a sprite.
  ''' The key is the field name and the value is the equivalent description,
  ''' as determined by the <see cref="DisplayAttribute.Description" />
  ''' that has been specified for the field.
  ''' </summary>
  Shared ReadOnly SpriteFieldDescriptions As ReadOnlyDictionary(Of String, String)

  ''' <summary>
  ''' Initializes the <see cref="SpriteSheetViewModel"/> class.
  ''' </summary>
  Shared Sub New()

    ' Iterate over all properties on the sprite sheet
    Dim sheetProps As Reflection.PropertyInfo() = GetType(SpriteSheet).GetProperties()
    Dim sheetFieldDict As New Dictionary(Of String, String)

    For Each sheetProp In sheetProps

      ' If we have a DisplayAttribute, stick the equivalent description
      ' in our dictionary for this property name.
      Dim valAtts As Object() =
        sheetProp.GetCustomAttributes(GetType(DisplayAttribute), True)

      If valAtts IsNot Nothing AndAlso valAtts.Any() Then
        sheetFieldDict(sheetProp.Name) = CType(valAtts(0), DisplayAttribute).Description
      End If

    Next

    ' Copy over our dictionary
    SheetFieldDescriptions = New ReadOnlyDictionary(Of String, String)(sheetFieldDict)

    ' Now do the same thing for the sprite
    Dim spriteProps As Reflection.PropertyInfo() = GetType(Sprite).GetProperties()
    Dim spriteFieldDict As New Dictionary(Of String, String)

    For Each spriteProp In spriteProps

      ' If we have a DisplayAttribute, stick the equivalent description
      ' in our dictionary for this property name.
      Dim valAtts As Object() =
        spriteProp.GetCustomAttributes(GetType(DisplayAttribute), True)

      If valAtts IsNot Nothing AndAlso valAtts.Any() Then
        spriteFieldDict(spriteProp.Name) = CType(valAtts(0), DisplayAttribute).Description
      End If

    Next

    ' Copy over our dictionary
    SpriteFieldDescriptions = New ReadOnlyDictionary(Of String, String)(spriteFieldDict)

  End Sub

  ''' <summary>
  ''' Initializes a new instance of the <see cref="SpriteSheetViewModel"/> class.
  ''' </summary>
  ''' <param name="spriteSheet">The sprite sheet.</param>
  ''' <param name="containingWindow">The containing window, to be used
  ''' for displaying any modal dialogs as necessary.</param>
  Public Sub New(spriteSheet As SpriteSheet,
                 containingWindow As Window)

    ' Set up our internals.
    Me._spriteSheet = spriteSheet
    Me._sprites = New ObservableCollection(Of Sprite)(spriteSheet.Sprites)
    Me._service = New SpriteSheetService(Me._sprites)
    Me._readOnlySprites = New ReadOnlyObservableCollection(Of Sprite)(Me._sprites)
    Me._containingWindow = containingWindow

    ' Wire up our commands
    Me._addCommand = New AddCommand(
      Me.Service,
      Function() Me.CanAdd,
      Sub(sprite)
        Me.CurrentSprite = sprite
        Me.PropagateSpriteCollectionChanges()
      End Sub)

    Me._deleteCommand = New DeleteCommand(
      Me.Service,
      Function() Me.CanDelete,
      Sub(sprite)
        Me.CurrentSprite = Nothing
        Me.PropagateSpriteCollectionChanges()
      End Sub)

    Me._moveUpCommand = New MoveUpCommand(
      Me.Service,
      Function() Me.CanMoveUp,
      Sub(sprite)
        Me.CanMoveChanging()
      End Sub,
      Sub(sprite)
        Me.CanMoveChanged()
        Me.PropagateSpriteCollectionChanges()
      End Sub)

    Me._moveDownCommand = New MoveDownCommand(
      Me.Service,
      Function() Me.CanMoveDown,
      Sub(sprite)
        Me.CanMoveChanging()
      End Sub,
      Sub(sprite)
        Me.CanMoveChanged()
        Me.PropagateSpriteCollectionChanges()
      End Sub)

    Me._browseImagePathCommand = New BrowseImagePathCommand(
      Me.Service,
      Function() Me.CanChangeImagePath,
      Nothing)

    Me._browseHoverImagePathCommand = New BrowseHoverImagePathCommand(
      Me.Service,
      Function() Me.CanChangeImagePath,
      Nothing)

    Me._clearImagePathCommand = New ClearImagePathCommand(
      Me.Service,
      Function() Me.CanChangeImagePath,
      Nothing)

    Me._clearHoverImagePathCommand = New ClearHoverImagePathCommand(
      Me.Service,
      Function() Me.CanChangeImagePath,
      Nothing)

    Me._newSpriteSheetCommand = New NewSpriteSheetCommand(
      Me.Service,
      Function() Me.CanSaveLoadSpriteSheet,
      Sub(sheet)
        ' Create a file information object with a blank name.
        RaiseEvent NewSpriteSheetLoaded(New SpriteSheetFileInformation(sheet, ""))
      End Sub)

    Me._loadSpriteSheetCommand = New LoadSpriteSheetCommand(
      Me.Service,
      Function() Me.CanSaveLoadSpriteSheet,
      Sub(sheetInformation)
        RaiseEvent NewSpriteSheetLoaded(sheetInformation)
      End Sub,
      Me.ContainingWindow)

    Me._saveSpriteSheetCommand = New SaveSpriteSheetCommand(
      Me.Service,
      Function() Me.CanSaveLoadSpriteSheet,
      Sub(sheetInformation)
        ' Indicate we no longer have unsaved changes.
        Me.HasUnsavedChanges = False

        ' Update our file name.
        Me.FileName = sheetInformation.FileName
      End Sub,
      Me.ContainingWindow)

    Me._saveSpriteSheetContentsCommand = New SaveSpriteSheetContentsCommand(
      Me.Service,
      Function() Me.CanSaveSpriteSheetContents,
      Nothing,
      Me.ContainingWindow)

    ' Add handlers for when our sprite sheet or sprite collection changes.
    AddHandler Me._spriteSheet.PropertyChanged, AddressOf SpriteSheetPropertyChangedHandler
    AddHandler Me._sprites.CollectionChanged, AddressOf SpriteCollectionChangedHandler

  End Sub

  ''' <summary>
  ''' Propagates the changes made to the <see cref="Sprites" />
  ''' collection to the <see cref="SpriteSheet">sprite sheet's</see>
  ''' <see cref="SpriteSheet.Sprites">sprites list</see>.
  ''' </summary>
  Private Sub PropagateSpriteCollectionChanges()
    Me.SpriteSheet.Sprites = Me.Sprites.ToList()
  End Sub

  Private ReadOnly _spriteSheet As SpriteSheet

  ''' <summary>
  ''' Gets the sprite sheet represented by this view model.
  ''' Note that changes to this instance's <see cref="SpriteSheet.Sprites" />
  ''' collection will be ignored. Any necessary changes should be made
  ''' to <see cref="Sprites" />.
  ''' </summary>
  ''' <value>
  ''' The sprite sheet represented by this view model.
  ''' </value>
  Public ReadOnly Property SpriteSheet() As SpriteSheet
    Get
      Return Me._spriteSheet
    End Get
  End Property

  Private ReadOnly _sprites As ObservableCollection(Of Sprite)
  Private ReadOnly _readOnlySprites As ReadOnlyObservableCollection(Of Sprite)

  ''' <summary>
  ''' Gets the sprites in the sheet represented by this view model.
  ''' </summary>
  ''' <value>
  ''' The sprites in the sheet represented by this view model.
  ''' </value>
  Public ReadOnly Property Sprites As ReadOnlyObservableCollection(Of Sprite)
    Get
      Return Me._readOnlySprites
    End Get
  End Property

  Private ReadOnly _service As SpriteSheetService

  ''' <summary>
  ''' Gets the service used by this view model.
  ''' </summary>
  ''' <value>
  ''' The service used by this view model.
  ''' </value>
  Public ReadOnly Property Service() As SpriteSheetService
    Get
      Return Me._service
    End Get
  End Property

  Private ReadOnly _containingWindow As Window

  ''' <summary>
  ''' Gets the containing window, to be used for displaying
  ''' any modal dialogs as necessary.
  ''' </summary>
  ''' <value>
  ''' The containing window, to be used for displaying any
  ''' modal dialogs as necessary.
  ''' </value>
  Public ReadOnly Property ContainingWindow() As Window
    Get
      Return Me._containingWindow
    End Get
  End Property

  Private _currentSprite As Sprite

  ''' <summary>
  ''' Gets or sets the currently-selected sprite.
  ''' </summary>
  ''' <value>
  ''' The currently-selected sprite.
  ''' </value>
  Public Property CurrentSprite() As Sprite
    Get
      Return Me._currentSprite
    End Get
    Set(value As Sprite)

      If value IsNot Me._currentSprite Then

        ' We're skipping CanAdd and some of the other
        ' always-true properties here.
        RaisePropertyChanging(Function() CurrentSprite)
        RaisePropertyChanging(Function() CanDelete)
        RaisePropertyChanging(Function() CanMoveUp)
        RaisePropertyChanging(Function() CanMoveDown)
        RaisePropertyChanging(Function() CanChangeImagePath)

        Me._currentSprite = value

        RaisePropertyChanged(Function() CurrentSprite)
        RaisePropertyChanged(Function() CanDelete)
        RaisePropertyChanged(Function() CanMoveUp)
        RaisePropertyChanged(Function() CanMoveDown)
        RaisePropertyChanged(Function() CanChangeImagePath)

      End If

    End Set
  End Property

  Private _fileName As String = "New sheet"

  ''' <summary>
  ''' Gets or sets the name of the file that was last used
  ''' to save/load the sprite sheet.
  ''' </summary>
  ''' <value>
  ''' The name of the file that was last used to
  ''' save/load the sprite sheet.
  ''' </value>
  Public Property FileName() As String
    Get
      Return Me._fileName
    End Get
    Set(value As String)

      If value <> Me._fileName Then

        RaisePropertyChanging(Function() FileName)
        RaisePropertyChanging(Function() SheetTitle)

        Me._fileName = value

        RaisePropertyChanged(Function() FileName)
        RaisePropertyChanged(Function() SheetTitle)

      End If

    End Set
  End Property

  ''' <summary>
  ''' Gets the title to use for the sprite sheet, which is dependent
  ''' on the <see cref="FileName">file name</see> and if there
  ''' are any <see cref="HasUnsavedChanges">unsaved changes</see>.
  ''' </summary>
  ''' <value>
  ''' The title to use for the sprite sheet.
  ''' </value>
  Public ReadOnly Property SheetTitle() As String
    Get

      ' Include an indicator if we have unsaved changes.
      If Me.HasUnsavedChanges = True Then
        Return "Tropical Sprite Sheet Builder - " & Me.FileName & "*"
      Else
        Return "Tropical Sprite Sheet Builder - " & Me.FileName
      End If

    End Get
  End Property

  Private _hasUnsavedChanges As Boolean = False

  ''' <summary>
  ''' Gets a value indicating whether the sprite sheet or one of
  ''' its sprites has unsaved changes.
  ''' </summary>
  ''' <value>
  ''' <c>true</c> if the sprite sheet or one of its sprites
  ''' has unsaved changes; otherwise, <c>false</c>.
  ''' </value>
  Public Property HasUnsavedChanges() As Boolean
    Get
      Return Me._hasUnsavedChanges
    End Get
    Friend Set(value As Boolean)

      If Me._hasUnsavedChanges <> value Then

        RaisePropertyChanging(Function() HasUnsavedChanges)
        RaisePropertyChanging(Function() SheetTitle)

        Me._hasUnsavedChanges = value

        RaisePropertyChanged(Function() HasUnsavedChanges)
        RaisePropertyChanged(Function() SheetTitle)

      End If

    End Set
  End Property

  ''' <summary>
  ''' Checks to see if there are any unsaved changes, and if so,
  ''' asks the user if they wish to discard them.
  ''' </summary>
  ''' <returns><c>true</c> if there are no unsaved changes
  ''' or the user has indicated they wish to discard them;
  ''' otherwise, <c>false</c>.</returns>
  Private Function PromptUnsavedChanges() As Boolean

    ' Show a prompt if we have unsaved changes.
    If Me.HasUnsavedChanges AndAlso
      MessageBox.Show("You have unsaved changes. Are you sure you wish to discard them?",
                       "Unsaved changes", MessageBoxButton.OKCancel,
                        MessageBoxImage.Question) = MessageBoxResult.Cancel Then
      Return False
    End If

    ' We're good.
    Return True

  End Function

  ''' <summary>
  ''' Gets the handler that will check for unsaved changes and prompt
  ''' the user if they wish to discard them. If there are no unsaved changes,
  ''' or the user wishes to discard them, the handler will return <c>true</c>;
  ''' otherwise, <c>false</c>.
  ''' </summary>
  ''' <value>
  ''' The handler that will check for unsaved changes and prompt
  ''' the user if they wish to discard them.
  ''' </value>
  Public ReadOnly Property CheckUnsavedChangesHandler() As Func(Of Boolean)
    Get
      Return AddressOf PromptUnsavedChanges
    End Get
  End Property

  ''' <summary>
  ''' Handles when the sprite collection has changed.
  ''' </summary>
  ''' <param name="sender">The sender.</param>
  ''' <param name="e">The <see cref="Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
  Private Sub SpriteCollectionChangedHandler(sender As Object, e As Specialized.NotifyCollectionChangedEventArgs)
    ' Indicate we have unsaved changes
    Me.HasUnsavedChanges = True
  End Sub

  ''' <summary>
  ''' Handles when a property on the sprite sheet has changed.
  ''' </summary>
  ''' <param name="sender">The sender.</param>
  ''' <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
  Private Sub SpriteSheetPropertyChangedHandler(sender As Object, e As PropertyChangedEventArgs)
    ' Indicate we have unsaved changes
    Me.HasUnsavedChanges = True
  End Sub

  ''' <summary>
  ''' Handles when a property has changed.  Used to simplify the process
  ''' of invoking <see cref="ICommand.CanExecuteChanged" /> when corresponding
  ''' properties (such as <see cref="CanDelete" />) are changed.
  ''' </summary>
  ''' <param name="sender">The sender.</param>
  ''' <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
  Private Sub PropertyChangedHandler(sender As Object, e As PropertyChangedEventArgs) Handles Me.PropertyChanged

    Select Case e.PropertyName.ToLower()

      Case "canadd"
        AddCommand.RaiseCanExecuteChanged()

      Case "candelete"
        DeleteCommand.RaiseCanExecuteChanged()

      Case "canmoveup"
        MoveUpCommand.RaiseCanExecuteChanged()

      Case "canmovedown"
        MoveDownCommand.RaiseCanExecuteChanged()

      Case "canchangeimagepath"
        BrowseImagePathCommand.RaiseCanExecuteChanged()
        BrowseHoverImagePathCommand.RaiseCanExecuteChanged()
        ClearImagePathCommand.RaiseCanExecuteChanged()
        ClearHoverImagePathCommand.RaiseCanExecuteChanged()

    End Select

  End Sub

#Region "Field Descriptions/Validation"

  ''' <summary>
  ''' Gets the description for a field on a sprite sheet.
  ''' </summary>
  ''' <param name="fieldName">The name of the field for which
  ''' a description will be retrieved</param>
  ''' <value>
  ''' The description for the field, or <c>String.Empty</c> if no
  ''' description is available.
  ''' </value>
  Public ReadOnly Property SheetDescriptions(fieldName As String) As String
    Get

      If SpriteSheetViewModel.SheetFieldDescriptions.ContainsKey(fieldName) Then
        Return SpriteSheetViewModel.SheetFieldDescriptions(fieldName)
      Else
        Return String.Empty
      End If

    End Get
  End Property

  ''' <summary>
  ''' Gets the description for a field on a sprite.
  ''' </summary>
  ''' <param name="fieldName">The name of the field for which
  ''' a description will be retrieved</param>
  ''' <value>
  ''' The description for the field, or <c>String.Empty</c> if no
  ''' description is available.
  ''' </value>
  Public ReadOnly Property SpriteDescriptions(fieldName As String) As String
    Get

      If SpriteSheetViewModel.SpriteFieldDescriptions.ContainsKey(fieldName) Then
        Return SpriteSheetViewModel.SpriteFieldDescriptions(fieldName)
      Else
        Return String.Empty
      End If

    End Get
  End Property

#End Region

#Region "Sprite Sheet Loading"

  ''' <summary>
  ''' Occurs when a new sprite sheet has been loaded,
  ''' which notifies listeners that a new view model
  ''' should be created from that sheet.
  ''' </summary>
  Public Event NewSpriteSheetLoaded(sheetInformation As SpriteSheetFileInformation)

#End Region

#Region "Command Implementation"

  ''' <summary>
  ''' Gets a value indicating whether sprites can be
  ''' added to the sheet.
  ''' </summary>
  ''' <value>
  ''' <c>true</c> if sprites can be added
  ''' to the sheet; otherwise, <c>false</c>.
  ''' </value>
  Public ReadOnly Property CanAdd() As Boolean
    Get
      ' Right now, we can always add sprites to the sheet
      Return True
    End Get
  End Property

  ''' <summary>
  ''' Gets a value indicating whether the current sprite can be
  ''' removed from the sheet.
  ''' </summary>
  ''' <value>
  ''' <c>true</c> if the current sprite can be removed
  ''' from the sheet; otherwise, <c>false</c>.
  ''' </value>
  Public ReadOnly Property CanDelete() As Boolean
    Get
      Return Me.CurrentSprite IsNot Nothing
    End Get
  End Property

  ''' <summary>
  ''' Gets a value indicating whether the current sprite can be
  ''' moved up in the sheet.
  ''' </summary>
  ''' <value>
  ''' <c>true</c> if the current sprite can be moved up
  ''' in the sheet; otherwise, <c>false</c>.
  ''' </value>
  Public ReadOnly Property CanMoveUp() As Boolean
    Get
      Return Me.CurrentSprite IsNot Nothing AndAlso
        Not Object.ReferenceEquals(Me.CurrentSprite, Me.Sprites.First())
    End Get
  End Property

  ''' <summary>
  ''' Gets a value indicating whether the current sprite can be
  ''' moved down in the sheet.
  ''' </summary>
  ''' <value>
  ''' <c>true</c> if the current sprite can be moved down
  ''' in the sheet; otherwise, <c>false</c>.
  ''' </value>
  Public ReadOnly Property CanMoveDown() As Boolean
    Get
      Return Me.CurrentSprite IsNot Nothing AndAlso
        Not Object.ReferenceEquals(Me.CurrentSprite, Me.Sprites.Last())
    End Get
  End Property

  ''' <summary>
  ''' Raises <see cref="PropertyChanging" /> events for the
  ''' <see cref="CanMoveUp" /> and <see cref="CanMoveDown" />
  ''' properties.
  ''' </summary>
  Private Sub CanMoveChanging()

    RaisePropertyChanging(Function() Me.CanMoveUp)
    RaisePropertyChanging(Function() Me.CanMoveDown)

  End Sub

  ''' <summary>
  ''' Raises <see cref="PropertyChanged" /> events for the
  ''' <see cref="CanMoveUp" /> and <see cref="CanMoveDown" />
  ''' properties.
  ''' </summary>
  Private Sub CanMoveChanged()

    RaisePropertyChanged(Function() Me.CanMoveUp)
    RaisePropertyChanged(Function() Me.CanMoveDown)

  End Sub

  ''' <summary>
  ''' Gets a value indicating whether the current sprite can have
  ''' its image paths changed.
  ''' </summary>
  ''' <value>
  ''' <c>true</c> if the current sprite can have its
  ''' image paths changed; otherwise, <c>false</c>.
  ''' </value>
  Public ReadOnly Property CanChangeImagePath() As Boolean
    Get
      Return Me.CurrentSprite IsNot Nothing
    End Get
  End Property

  ''' <summary>
  ''' Gets a value indicating whether a sprite sheet
  ''' can be saved or loaded.
  ''' </summary>
  ''' <value>
  ''' <c>true</c> if a sprite sheet can be saved or loaded; 
  ''' otherwise, <c>false</c>.
  ''' </value>
  Public ReadOnly Property CanSaveLoadSpriteSheet() As Boolean
    Get
      ' Right now, we can always save/load.
      Return True
    End Get
  End Property

  ''' <summary>
  ''' Gets a value indicating whether the current sprite
  ''' sheet's contents (CSS, sprite image, and sample HTML)
  ''' can be generated.
  ''' </summary>
  ''' <value>
  ''' <c>true</c> if the current sprite sheet's contents can be
  ''' generated; otherwise, <c>false</c>.
  ''' </value>
  Public ReadOnly Property CanSaveSpriteSheetContents() As Boolean
    Get
      ' Right now, we can always save sprite sheet contents.
      Return True
    End Get
  End Property

  Private ReadOnly _addCommand As AddCommand

  ''' <summary>
  ''' Gets the command used to add a new sprite.
  ''' </summary>
  ''' <value>
  ''' The command used to add a new sprite.
  ''' </value>
  Public ReadOnly Property AddCommand As AddCommand
    Get
      Return Me._addCommand
    End Get
  End Property

  Private ReadOnly _deleteCommand As DeleteCommand

  ''' <summary>
  ''' Gets the command used to delete a sprite.
  ''' </summary>
  ''' <value>
  ''' The command used to delete a sprite.
  ''' </value>
  Public ReadOnly Property DeleteCommand As DeleteCommand
    Get
      Return Me._deleteCommand
    End Get
  End Property

  Private ReadOnly _moveUpCommand As MoveUpCommand

  ''' <summary>
  ''' Gets the command used to move up a sprite in the list.
  ''' </summary>
  ''' <value>
  ''' The command used to move up a sprite in the list.
  ''' </value>
  Public ReadOnly Property MoveUpCommand As MoveUpCommand
    Get
      Return Me._moveUpCommand
    End Get
  End Property

  Private ReadOnly _moveDownCommand As MoveDownCommand

  ''' <summary>
  ''' Gets the command used to move down a sprite in the list.
  ''' </summary>
  ''' <value>
  ''' The command used to move down a sprite in the list.
  ''' </value>
  Public ReadOnly Property MoveDownCommand As MoveDownCommand
    Get
      Return Me._moveDownCommand
    End Get
  End Property

  Private ReadOnly _browseImagePathCommand As BrowseImagePathCommand

  ''' <summary>
  ''' Gets the command used to browse for a file to use
  ''' as a sprite's <see cref="Sprite.ImagePath" />.
  ''' </summary>
  ''' <value>
  ''' The command used to browse for a sprite's <see cref="Sprite.ImagePath" />.
  ''' </value>
  Public ReadOnly Property BrowseImagePathCommand As BrowseImagePathCommand
    Get
      Return Me._browseImagePathCommand
    End Get
  End Property

  Private ReadOnly _browseHoverImagePathCommand As BrowseHoverImagePathCommand

  ''' <summary>
  ''' Gets the command used to browse for a file to use
  ''' as a sprite's <see cref="Sprite.HoverImagePath" />.
  ''' </summary>
  ''' <value>
  ''' The command used to browse for a sprite's <see cref="Sprite.HoverImagePath" />.
  ''' </value>
  Public ReadOnly Property BrowseHoverImagePathCommand As BrowseHoverImagePathCommand
    Get
      Return Me._browseHoverImagePathCommand
    End Get
  End Property

  Private ReadOnly _clearImagePathCommand As ClearImagePathCommand

  ''' <summary>
  ''' Gets the command used to browse for a file to clear out
  ''' a sprite's <see cref="Sprite.ImagePath" />.
  ''' </summary>
  ''' <value>
  ''' The command used to clear out a sprite's <see cref="Sprite.ImagePath" />.
  ''' </value>
  Public ReadOnly Property ClearImagePathCommand As ClearImagePathCommand
    Get
      Return Me._clearImagePathCommand
    End Get
  End Property

  Private ReadOnly _clearHoverImagePathCommand As ClearHoverImagePathCommand

  ''' <summary>
  ''' Gets the command used to browse for a file to clear out
  ''' a sprite's <see cref="Sprite.HoverImagePath" />.
  ''' </summary>
  ''' <value>
  ''' The command used to clear out a sprite's <see cref="Sprite.HoverImagePath" />.
  ''' </value>
  Public ReadOnly Property ClearHoverImagePathCommand As ClearHoverImagePathCommand
    Get
      Return Me._clearHoverImagePathCommand
    End Get
  End Property

  Private ReadOnly _newSpriteSheetCommand As NewSpriteSheetCommand

  ''' <summary>
  ''' Gets the command used to create a new sprite sheet.
  ''' </summary>
  ''' <value>
  ''' The command used to create a new sprite sheet.
  ''' </value>
  Public ReadOnly Property NewSpriteSheetCommand As NewSpriteSheetCommand
    Get
      Return Me._newSpriteSheetCommand
    End Get
  End Property

  Private ReadOnly _loadSpriteSheetCommand As LoadSpriteSheetCommand

  ''' <summary>
  ''' Gets the command used to browse for and load up
  ''' a new sprite sheet.
  ''' </summary>
  ''' <value>
  ''' The command used to browse for and load up a new sprite sheet.
  ''' </value>
  Public ReadOnly Property LoadSpriteSheetCommand As LoadSpriteSheetCommand
    Get
      Return Me._loadSpriteSheetCommand
    End Get
  End Property

  Private _saveSpriteSheetCommand As SaveSpriteSheetCommand

  ''' <summary>
  ''' Gets the command used to browse to save the current sprite sheet
  ''' to a file.
  ''' </summary>
  ''' <value>
  ''' The command used to save the current sprite sheet
  ''' to a file.
  ''' </value>
  Public ReadOnly Property SaveSpriteSheetCommand() As SaveSpriteSheetCommand
    Get
      Return _saveSpriteSheetCommand
    End Get
  End Property

  Private _saveSpriteSheetContentsCommand As SaveSpriteSheetContentsCommand

  ''' <summary>
  ''' Gets the command used to browse to generate the contents
  ''' of the current sprite sheet.
  ''' </summary>
  ''' <value>
  ''' The command used to generate the contents of the current sprite sheet.
  ''' </value>
  Public ReadOnly Property SaveSpriteSheetContentsCommand() As SaveSpriteSheetContentsCommand
    Get
      Return _saveSpriteSheetContentsCommand
    End Get
  End Property

#End Region

End Class
