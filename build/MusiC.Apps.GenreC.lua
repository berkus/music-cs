-- MusiC.Apps.GenreC

package = newpackage()
package.name="MusiC.Apps.GenreC"
package.path=project.path

-- Input
package.language="c#"
package.files={
	matchrecursive(base_src_dir.."/Apps/GenreC/*.cs")
}
package.links={
	"System",
	"MusiC"
}
-- Code Generation
package.defines={"TRACE"}
--package.buildflags={"unsafe"}

-- Output
package.kind="exe"
package.target="MusiC.Apps.GenreC"
package.targetprefix=""
package.targetextension="dll"
package.objdir=base_bin_dir.."/obj"

-- Debug:MusiC
package.config["Debug"].defines={"DEBUG"}
