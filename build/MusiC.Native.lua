-- MusiC.Extensions.Classifiers.uBarbedo

package = newpackage()
package.name="MusiC.Native"
package.path=project.path

-- Input
package.language="c++"
package.files={
	matchrecursive(base_src_dir.."/Native/*.cpp"),
	matchrecursive(base_src_dir.."/Native/*.h")
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
package.buildflags={"dylib"}

-- Output
package.kind="dll"
package.target="MusiC.Native"
package.targetprefix=""
package.targetextension="dll"
package.bindir=base_bin_dir
package.objdir=base_bin_dir.."/obj"

-- Debug:MusiC
package.config["Debug"].defines={"DEBUG"}
