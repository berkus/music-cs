-- MusiC.Extensions.Handlers.uDBHandler

package = newpackage()
package.name="MusiC.Extensions.Handlers.uDBHandler"
package.path=project.path

-- Input
package.language="c++"
package.files={
	matchrecursive(base_src_dir.."/Extensions/Handlers/DBHandler/*.cpp"),
	matchrecursive(base_src_dir.."/Extensions/Handlers/DBHandler/*.h")
}
package.links={
	"gsl",
	"gslcblas"
}
package.libpaths={
	base_deps_dir.."/"..OS.."/lib"
}
package.includepaths={
	base_deps_dir.."/include"
}

-- Code Generation
--package.defines={"TRACE"}
package.buildflags={"dylib", "no-import-lib"}

-- Output
package.kind="dll"
package.target="MusiC.Extensions.Handlers.uDBHandler"
package.targetprefix=""
package.targetextension="dll"
package.bindir=base_bin_dir.."/Extensions"
package.objdir=base_bin_dir.."/obj"

-- Debug:MusiC
package.config["Debug"].defines={"DEBUG"}
