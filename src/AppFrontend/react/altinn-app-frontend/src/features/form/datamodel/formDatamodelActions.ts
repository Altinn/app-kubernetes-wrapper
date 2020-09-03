import { ActionCreatorsMapObject, bindActionCreators, Action } from 'redux';
import { store } from '../../../store';

import * as fetchActions from './fetch/fetchFormDatamodelActions';

interface IFormDataModelActions extends ActionCreatorsMapObject {
  fetchDataModel: (url: string) => fetchActions.IFetchDataModel;
  fetchDataModelFulfilled: (dataModel: any) => fetchActions.IFetchDataModelFulfilled;
  fetchDataModelRejected: (error: Error) => fetchActions.IFetchDataModelRejected;
  fetchJsonSchema: () => Action;
  fetchJsonSchemaFulfilled: (schema: any, id: string) => fetchActions.IFetchJsonSchemaFulfilled;
  fetchJsonSchemaRejected: (error: Error) => fetchActions.IFetchDataModelRejected;
}

const actions: IFormDataModelActions = {
  fetchDataModel: fetchActions.fetchDataModel,
  fetchDataModelFulfilled: fetchActions.fetchDataModelFulfilled,
  fetchDataModelRejected: fetchActions.fetchDataModelRejected,
  fetchJsonSchema: fetchActions.fetchJsonSchema,
  fetchJsonSchemaFulfilled: fetchActions.fetchJsonSchemaFulfilled,
  fetchJsonSchemaRejected: fetchActions.fetchJsonSchemaRejected,
};

const DataModelActions: IFormDataModelActions = bindActionCreators<any, any>(actions, store.dispatch);

export default DataModelActions;
