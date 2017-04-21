# Use meshes in Unity UI

This component scales meshes so that they fit into a UI Rect. Canvas render mode can't be ScreenSpaceOverlay and the set-up should be like this:

--GameObject (has the CanvasElement3D component, the UI Rect defines the scale)
----Scaler (empty GameObject (this is so that the 3D Element can be made up of multiple meshes))
------Actual Meshes (the Meshes that should be scaled, drag all the meshes that should be considered into the rendersToScale property on the CanvasElement3D)

## License
Licensed under a MIT-License.
