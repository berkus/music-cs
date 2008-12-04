-- MusiC.Extensions.Configs.XMLConfig

package = newpackage()
package.name="MusiC.Extensions.Configs.XMLConfig"

-- Input
package.language="c#"
package.files={
	matchrecursive("../src/Extensions/Configs/XMLConfig/*.cs")
}
package.links={
	"System",
	"System.Xml",
	"MusiC"
}
-- Code Generation
package.defines={"TRACE"}
package.buildflags={"unsafe"}

-- Output
package.kind="dll"
package.target="MusiC.Extensions.Configs.XMLConfig"
package.targetprefix=""
package.targetextension="dll"
package.objdir="../bin/obj"

-- Debug:MusiC
package.config["Debug"].defines={"DEBUG"}
