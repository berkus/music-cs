-- MusiC.Extensions.Handlers.DBHandler

package = newpackage()
package.name="MusiC.Extensions.Handlers.DBHandler"

-- Input
package.language="c#"
package.files={
	matchrecursive("../src/Extensions/Handlers/DBHandler/*.cs")
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
package.target="MusiC.Extensions.Handlers.DBHandler"
package.targetprefix=""
package.targetextension="dll"
package.objdir="../bin/obj"

-- Debug:MusiC
package.config["Debug"].defines={"DEBUG"}
