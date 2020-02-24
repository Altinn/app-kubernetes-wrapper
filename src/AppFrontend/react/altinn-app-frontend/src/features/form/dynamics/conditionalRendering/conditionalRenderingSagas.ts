import { SagaIterator } from 'redux-saga';
import { all, call, select, take, takeLatest } from 'redux-saga/effects';
import { IRuntimeState } from '../../../../types';
import { IValidations } from '../../../../types/global';
import { runConditionalRenderingRules } from '../../../../utils/conditionalRendering';
import * as FormConfigActionTypes from '../../config/fetch/fetchFormConfigActionTypes';
import * as FormDataActionTypes from '../../data/formDataActionTypes';
import { IFormData } from '../../data/formDataReducer';
import { ILayout } from '../../layout';
import FormLayoutActions from '../../layout/formLayoutActions';
import * as FormLayoutActionTypes from '../../layout/formLayoutActionTypes';
import FormValidationActions from '../../validation/validationActions';
import * as FormDynamicsActionTypes from '../formDynamicsActionTypes';
import { IConditionalRenderingRules } from '../types';

export const ConditionalRenderingSelector:
  (store: IRuntimeState) => any = (store: IRuntimeState) => store.formDynamics.conditionalRendering;
export const FormDataSelector: (store: IRuntimeState) => IFormData = (store) => store.formData.formData;
export const FormLayoutSelector: (store: IRuntimeState) => ILayout = (store) => store.formLayout.layout;
export const FormValidationSelector: (store: IRuntimeState) =>
  IValidations = (store) => store.formValidations.validations;

function* checkIfConditionalRulesShouldRunSaga(): SagaIterator {
  try {
    const conditionalRenderingState: IConditionalRenderingRules = yield select(ConditionalRenderingSelector);
    const formData: IFormData = yield select(FormDataSelector);
    const formValidations: IValidations = yield select(FormValidationSelector);
    const componentsToHide: string[] = runConditionalRenderingRules(
        conditionalRenderingState,
        formData,
    );

    if (componentsToHide.length > 0) {
      FormLayoutActions.updateHiddenComponents(componentsToHide);
      componentsToHide.forEach((componentId) => {
        if (formValidations[componentId]) {
          const newFormValidations = formValidations;
          delete formValidations[componentId];
          FormValidationActions.updateValidations(newFormValidations);
        }
      });
    }
    
  } catch (err) {
    yield call(console.error, err);
  }
}

export function* watchCheckIfConditionalRulesShouldRunSaga(): SagaIterator {
  yield takeLatest(FormDynamicsActionTypes.CHECK_IF_CONDITIONAL_RULE_SHOULD_RUN, checkIfConditionalRulesShouldRunSaga);
}

export function* waitForAppSetupBeforeRunningConditionalRulesSaga(): SagaIterator {
  yield all([
    take(FormLayoutActionTypes.FETCH_FORM_LAYOUT_FULFILLED),
    take(FormConfigActionTypes.FETCH_FORM_CONFIG_FULFILLED),
    take(FormDataActionTypes.FETCH_FORM_DATA_FULFILLED),
    take(FormDynamicsActionTypes.FETCH_SERVICE_CONFIG_FULFILLED),
  ]);
  yield call(checkIfConditionalRulesShouldRunSaga, {});
}
