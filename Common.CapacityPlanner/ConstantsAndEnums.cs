using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.CapacityPlanner
{
	#region Enums
	#endregion Enums

	public class Constants
	{
		#region Constants for DB Lookups
		//NOTE: Coding these in here as they should be relatively static, and our DB Model doesn't do lookups for these
		//so we don't need to go to the DB to resolve them.
		public const string AllocationTypePlanned = "Planned";
		public const string AllocationTypeActual = "Actual";

		public const string EmploymentTypeFTE = "FTE";
		public const string EmploymentTypeContingent = "Contingent";

		public const string ImplementationCategoryImplementation = "Implementation";
		public const string ImplementationCategoryIntegration = "Integration";
		public const string ImplementationCategoryPOC = "POC";
		public const string ImplementationCategoryProduct = "Product";

		public const string StatusActive = "Active";
		public const string StatusClosed = "Closed";
		public const string StatusEvergreen = "Evergreen";
		public const string StatusPlanned = "Planned";
		public const string StatusRetired = "Retired";

		public const string WorkstreamCategoryEnhancement = "Enhancement";
		public const string WorkstreamCategoryImplementation = "Implementation";
		public const string WorkstreamCategoryProductDevelopment = "ProductDevelopment";
		public const string WorkstreamCategoryResearchAndDevelopment = "R&D";
		public const string WorkstreamCategorySupport = "Support";

		#endregion Constants for DB Lookups
	}
}
