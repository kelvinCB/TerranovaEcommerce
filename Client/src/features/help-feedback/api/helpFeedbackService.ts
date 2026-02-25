import { HelpFeedbackPayload } from "../types";

const HELP_FEEDBACK_ENDPOINTS = {
  "bug-report": "/api/help-feedback/bug-report",
  "feature-suggestion": "/api/help-feedback/feature-suggestion",
} as const;

export async function submitHelpFeedback(
  type: keyof typeof HELP_FEEDBACK_ENDPOINTS,
  payload: HelpFeedbackPayload,
) {
  const response = await fetch(HELP_FEEDBACK_ENDPOINTS[type], {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(payload),
  });

  if (!response.ok) {
    throw new Error("No pudimos enviar tu solicitud. Inténtalo de nuevo.");
  }

  return response.json().catch(() => ({ success: true }));
}

export { HELP_FEEDBACK_ENDPOINTS };
