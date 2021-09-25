using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PluginHomeWorkBonus
{
	public class PluginTest : IPlugin
	{
		public void Execute(IServiceProvider serviceProvider)
		{
			IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
			IOrganizationServiceFactory organizationServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
			IOrganizationService organizationService = organizationServiceFactory.CreateOrganizationService(context.UserId);

			if (context.InputParameters.Contains("EntityMoniker") && context.InputParameters["EntityMoniker"] is EntityReference && ((EntityReference)context.InputParameters["EntityMoniker"]).LogicalName.Equals(el_class.EntityLogicalName))
			{
				OptionSetValue state = (OptionSetValue)context.InputParameters["State"];
				if(state.Value == 0)
				{
					return;
				}

				EntityReference elClass = (EntityReference)context.InputParameters["EntityMoniker"];

				IEnumerable relatedStudents = GetEntityCollection(organizationService, el_currentstudent.EntityLogicalName, "el_currentstudentclass", elClass.Id.ToString(), new ColumnSet(new string[] { "el_currentstudentclass" }));
				foreach (Entity student in relatedStudents)
				{
					Entity updateEntity = new Entity(el_currentstudent.EntityLogicalName);
					updateEntity.Id = student.Id;
					updateEntity["el_currentstudentclass"] = null;

					organizationService.Update(updateEntity);


					//student["el_currentstudentclass"] = null;
					//organizationService.Update(student);
				}


				//QueryExpression query = new QueryExpression(el_currentstudent.EntityLogicalName) { ColumnSet = new ColumnSet("el_currentstudentclass") };
				//query.Criteria.AddCondition("el_currentstudentclass", ConditionOperator.Equal, elClass.Id);
				//EntityCollection relatedStudents = organizationService.RetrieveMultiple(query);

				//if (relatedStudents != null && relatedStudents.Entities != null && relatedStudents.Entities.Any())
				//{

				//	foreach (Entity student in relatedStudents.Entities)
				//	{
				//		student["el_currentstudentclass"] = null;
				//		organizationService.Update(student);
				//	}
				//}
			}
		}

		private IEnumerable GetEntityCollection(IOrganizationService service, string entityName, string attributeName, string attributeValue, ColumnSet cols)
		{
			QueryExpression query = new QueryExpression();
			query.EntityName = entityName;
			query.ColumnSet = cols;
			query.Criteria.AddCondition(new ConditionExpression(attributeName, ConditionOperator.Equal, attributeValue));
			RetrieveMultipleRequest request = new RetrieveMultipleRequest();
			request.Query = query;
			IEnumerable<Entity> entities = ((RetrieveMultipleResponse)service.Execute(request)).EntityCollection.Entities;
			return entities;
		}
	}
}
