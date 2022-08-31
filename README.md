# Geometrician and Lineweights

## Geometrician

Geometrician is an application to execute and visualise the results of geometric workflows using Hypar's Elements library.

The application is can run client-side directly in the browser using [Blazor WebAssembly](https://docs.microsoft.com/en-gb/aspnet/core/blazor/hosting-models?view=aspnetcore-6.0#blazor-server), or as a [Blazor Server](https://docs.microsoft.com/en-gb/aspnet/core/blazor/hosting-models?view=aspnetcore-6.0#blazor-server). Soon it will also be available as a [Blazor Hybrid](https://docs.microsoft.com/en-gb/aspnet/core/blazor/hosting-models?view=aspnetcore-6.0#blazor-hybrid) app using either .NET MAUI or WPF, and via a command line interface.

*View the WebAssembly App:*

*→ [Production (main branch)](https://geometrician.io/)*

*→ [Staging (develop branch)](https://next.geometrician.io/)*

---

📇 **Geometrician.Core**

![](https://img.shields.io/badge/status-alpha-informational)

The core [Razor components](https://docs.microsoft.com/en-us/aspnet/core/blazor/components/class-libraries?view=aspnetcore-6.0&tabs=visual-studio) for the Blazor application implementations.

*→ [Documentation](https://docs.lineweights.io/latest/App/Geometrician.Core.html)*
/  *[Source](https://github.com/StudioLE/Lineweights/tree/main/Geometrician.Core/src)*

---

🖧 **Geometrician.Server**

![](https://img.shields.io/badge/status-alpha-informational)

The [Blazor Server](https://docs.microsoft.com/en-gb/aspnet/core/blazor/hosting-models?view=aspnetcore-6.0#blazor-server) application.

*→ [Documentation](https://docs.lineweights.io/latest/App/Geometrician.Server.html)*
/  *[Source](https://github.com/StudioLE/Lineweights/tree/main/Geometrician.Server/src)*

---

🪟 **Geometrician.WebAssembly**

![](https://img.shields.io/badge/status-alpha-informational)

The [Blazor WebAssembly](https://docs.microsoft.com/en-gb/aspnet/core/blazor/hosting-models?view=aspnetcore-6.0#blazor-server) application.

*→ [Documentation](https://docs.lineweights.io/latest/Core/Lineweights.WebAssembly.html)*
/  *[Source](https://github.com/StudioLE/Lineweights/tree/main/Geometrician.WebAssembly/src)*
/  *[Live (main)](https://geometrician.io/)*
/  *[Live (develop)](https://next.geometrician.io/)*

---

## Lineweights

Lineweights is a collection of modular libraries for automating architecture, engineering, and construction (AEC) workflows.

*The components are written in C# using Hypar's Elements library. They are designed to be cross-platform for use in standard industry software and cloud platforms such as Hypar, Dynamo, Revit, Grasshopper, and Rhino.*

*Rather than solving specific AEC automation problems the intention of these libraries is to provide a base from which developers can build upon to solve AEC automation problems quicker, easier, and more reliably. The libraries are heavily documented, unit tested, and with specific samples demonstrating the use.*

---

### Core

Libraries containing generic code reused across Lineweights.

---

🧰 **Lineweights.Core**

![](https://img.shields.io/badge/status-alpha-informational)

Generic extension and helper methods for the Elements library.

*→ [Documentation](https://docs.lineweights.io/latest/Core/Lineweights.Core.html)*
/  *[Source](https://github.com/StudioLE/Lineweights/tree/main/Lineweights.Core/src)*

---

🔩 **StudioLE.Core**

![](https://img.shields.io/badge/status-alpha-informational)

Generic extension and helper methods for the C# language.

*→ [Documentation](https://docs.lineweights.io/latest/Core/StudioLE.Core.html)*
/  *[Source](https://github.com/StudioLE/Lineweights/tree/main/StudioLE.Core/src)*

---

### Distribution

Libraries that distribute or layout geometric elements.

---

📐 **Lineweights.Flex**

![](https://img.shields.io/badge/status-alpha-informational)

Arrange, position, align, place, repeat, array, and distribute geometric elements of different sizes inside a cuboid container *(for example a building, room, or car park)* or along a curve *(e.g. a road, train track, or linear structure)*.

*Typical distribution tools only work with elements of uniform size but this library follows the concepts of [Flexbox](https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_Flexible_Box_Layout/Basic_Concepts_of_Flexbox) allowing complete flexibility to arrange elements of any size together.*

*→ [Documentation](https://docs.lineweights.io/latest/Distribution/Lineweights.Flex.html)*
/  *[Source](https://github.com/StudioLE/Lineweights/tree/main/Lineweights.Flex/src)*
/ *[Samples](https://github.com/StudioLE/Lineweights/tree/main/Lineweights.Flex/samples)*

---

### Drawings

Libraries that provide drawing, sheet, and visualisation of geometric elements, as well as converison to other visual formats.

---

✏️ **Lineweights.Drawings**

![](https://img.shields.io/badge/status-alpha-informational)

2d view and sheet creation for the Elements library.

*→ [Documentation](https://docs.lineweights.io/latest/Drawings/Lineweights.Drawings.html)*
/  *[Source](https://github.com/StudioLE/Lineweights/tree/main/Lineweights.Drawings/src)*

---

📃 **Lineweights.PDF**

![](https://img.shields.io/badge/status-alpha-informational)

Convert geometric elements to 2d PDF visual representatons.

*→ [Documentation](https://docs.lineweights.io/latest/Drawings/Lineweights.PDF.html)*
/  *[Source](https://github.com/StudioLE/Lineweights/tree/main/Lineweights.PDF/src)*

---

🖼 **Lineweights.SVG**

![](https://img.shields.io/badge/status-alpha-informational)

Convert geometric elements to 2d SVG visual representatons.

*→ [Documentation](https://docs.lineweights.io/latest/Drawings/Lineweights.SVG.html)*
/  *[Source](https://github.com/StudioLE/Lineweights/tree/main/Lineweights.SVG/src)*

---

### Experimental

Experimental libraries.

---

📈 **Lineweights.Curves**

![](https://img.shields.io/badge/status-experimental-critical)

Curve interpolation and spline geometry for the Elements library.

*Typical architectural and urban planning software is limited to the rigidity of the grid street plan favoured by the US but elsewhere in the world the built environment has evolved more fluidly and pleasingly following the curvature of natural features. This library adds the missing curves that are essential to masterplanning.*

*→ [Documentation](https://docs.lineweights.io/latest/Geometry/Lineweights.Curves.html)*
/  *[Source](https://github.com/StudioLE/Lineweights/tree/main/Lineweights.Curves/src)*

---

🏘 **Lineweights.Masterplanning**

![](https://img.shields.io/badge/status-experimental-critical)

Automate the creation of residential capacity studies and masterplans.

*Typical masterplanning tools have strict limitations, working only with straight roads, cuboid buildings, or limited unit types but this library builds on the author's experience developing residential masterplans in architectural practice ensuring that it is flexible enough to work how you work.*

*→ [Documentation](https://docs.lineweights.io/latest/Masterplanning/Lineweights.Masterplanning.html)*
/  *[Source](https://github.com/StudioLE/Lineweights/tree/main/Lineweights.Masterplanning/src)*

---

### Workflows

Libraries that provide additional mechanisms for visualising and verifying the results of Elements logic.

---

📡 **Geometrician**

![](https://img.shields.io/badge/status-alpha-informational)

A [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) client application to visualise the results of Elements logic.

*→ [Documentation](https://docs.lineweights.io/latest/Workflows/Geometrician.html)*
/  *[Source](https://github.com/StudioLE/Lineweights/tree/main/Geometrician/src)*

---

🧪 **Lineweights.Workflows**

![](https://img.shields.io/badge/status-alpha-informational)

Visualise and verify the results of Elements logic.

*→ [Documentation](https://docs.lineweights.io/latest/Workflows/Lineweights.Workflows.html)*
/  *[Source](https://github.com/StudioLE/Lineweights/tree/main/Lineweights.Workflows/src)*

---

🔬 **Lineweights.Workflows.NUnit**

![](https://img.shields.io/badge/status-alpha-informational)

Visualise and verify the results of Elements logic in NUnit tests.

*→ [Documentation](https://docs.lineweights.io/latest/Workflows/Lineweights.Workflows.NUnit.html)*
/  *[Source](https://github.com/StudioLE/Lineweights/tree/main/Lineweights.Workflows.NUnit/src)*

---

## License

Geometrician and Lineweights are dual-licensed under open and closed source licenses.

Copyright © Laurence Elsdon 2022

### Open Source

This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.

→ [GNU Affero General Public License](https://github.com/StudioLE/Lineweights/tree/main/COPYING.md)

### Proprietary

The GNU Affero General Public License requires that you must disclose your source code when you distribute, publish or provide access to modified or derivative software therefore developers who wish to keep their projects proprietary or closed source under these circumstances should:

→ [Get in touch regarding commercial license agreements](https://studiole.uk/contact/)


