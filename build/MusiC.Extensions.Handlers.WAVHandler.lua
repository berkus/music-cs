-- MusiC.Extensions.Handlers.WAVHandler

package = newpackage()
package.name="MusiC.Extensions.Handlers.WAVHandler"

-- Input
package.language="c#"
package.files={
	matchrecursive("../src/Extensions/Handlers/WAVHandler/*.cs")
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
package.target="MusiC.Extensions.Handlers.WAVHandler"
package.targetprefix=""
package.targetextension="dll"
package.objdir="../bin/obj"

-- Debug:MusiC
package.config["Debug"].defines={"DEBUG"}
