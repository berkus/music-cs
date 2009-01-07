-- MusiC.Extensions.Configs.XMLConfig

package = newpackage()
package.name="MusiC.Extensions.Configs.XMLConfigurator"
package.path=project.path

-- Input
package.language="c#"
package.files={
	matchrecursive(base_src_dir.."/Extensions/Configs/XMLConfigurator/*.cs")
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
package.target="MusiC.Extensions.Configs.XMLConfigurator"
package.targetprefix=""
package.targetextension="dll"
package.bindir=base_bin_dir
package.objdir=base_bin_dir.."/obj"

-- Debug:MusiC
package.config["Debug"].defines={"DEBUG"}
