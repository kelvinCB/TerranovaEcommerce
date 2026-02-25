export type HelpFeedbackTab = "faq" | "bug-report" | "feature-suggestion";

export interface HelpFeedbackPayload {
  name: string;
  email: string;
  message: string;
  orderId?: string;
}
