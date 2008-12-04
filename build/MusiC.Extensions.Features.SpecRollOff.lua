-- MusiC.Extensions.Features.SpecRollOff

package = newpackage()
package.name="MusiC.Extensions.Features.SpecRollOff"

-- Input
package.language="c#"
package.files={
	matchrecursive("../src/Extensions/Features/SpecRollOff/*.cs")
}
package.links={
	"System",
	"MusiC"
}
-- Code Generation
package.defines={"TRACE"}
package.buildflags={"unsafe"}

-- Output
package.kind="dll"
package.target="MusiC.Extensions.Features.SpecRollOff"
package.targetprefix=""
package.targetextension="dll"
package.objdir="../bin/obj"

-- Debug:MusiC
package.config["Debug"].defines={"DEBUG"}