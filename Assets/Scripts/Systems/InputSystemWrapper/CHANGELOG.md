# Input System Wrapper
## Changelog

1.1.1
- Input contexts now support event system action overrides.
- Debugger window expanded to help see top-level view of system at runtime during play.
- Event System global options specified in offline input data.

1.1.0
- Actions are now accessed via one more layer of wrapping. This makes more sense to read. E.g.: Input.UI.OnClick event becomes Input.UI.Click.OnEvent, to fit with other functionality inside Input.UI.Click.
- ActionWrapper now supports buttons, values and pass-throughs, with explicitly-typed ReadValue methods.

1.0.0
- Initial version