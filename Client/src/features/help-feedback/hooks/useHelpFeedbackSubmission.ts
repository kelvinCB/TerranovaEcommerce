import { useMutation } from "@tanstack/react-query";
import { submitHelpFeedback } from "../api/helpFeedbackService";
import { HelpFeedbackPayload, HelpFeedbackTab } from "../types";

interface SubmitInput {
  type: Exclude<HelpFeedbackTab, "faq">;
  payload: HelpFeedbackPayload;
}

export function useHelpFeedbackSubmission() {
  return useMutation({
    mutationFn: ({ type, payload }: SubmitInput) => submitHelpFeedback(type, payload),
  });
}
