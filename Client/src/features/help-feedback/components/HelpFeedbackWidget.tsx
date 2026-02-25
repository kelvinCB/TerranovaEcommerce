import { useMemo, useState } from "react";
import { useForm } from "react-hook-form";
import { Bug, Lightbulb, MessageCircleQuestion, X } from "lucide-react";
import { useHelpFeedbackSubmission } from "../hooks/useHelpFeedbackSubmission";
import { HelpFeedbackTab } from "../types";

type FormValues = {
  name: string;
  email: string;
  orderId?: string;
  message: string;
};

const tabs: { key: HelpFeedbackTab; label: string }[] = [
  { key: "faq", label: "FAQ" },
  { key: "bug-report", label: "Bug Report" },
  { key: "feature-suggestion", label: "Feature Suggestion" },
];

const faqItems = [
  {
    question: "¿Cómo rastreo mi orden?",
    answer:
      "Ve a tu cuenta > órdenes. Ahí verás el estado en tiempo real y número de seguimiento.",
  },
  {
    question: "¿Puedo cambiar dirección luego del checkout?",
    answer:
      "Sí, mientras la orden no haya sido despachada. Escríbenos por Bug Report con tu número de orden.",
  },
  {
    question: "¿Cuál es el tiempo de entrega?",
    answer:
      "Generalmente entre 2 y 5 días laborables, según ubicación y disponibilidad.",
  },
];

function HelpFeedbackWidget() {
  const [isOpen, setIsOpen] = useState(false);
  const [activeTab, setActiveTab] = useState<HelpFeedbackTab>("faq");

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<FormValues>();

  const submissionMutation = useHelpFeedbackSubmission();

  const isFormTab = activeTab !== "faq";

  const formTitle = useMemo(() => {
    if (activeTab === "bug-report") return "Reporta un bug";
    if (activeTab === "feature-suggestion") return "Sugiere una mejora";
    return "Centro de ayuda";
  }, [activeTab]);

  const formDescription = useMemo(() => {
    if (activeTab === "bug-report") {
      return "Cuéntanos qué pasó, cuándo ocurrió y cómo podemos reproducirlo.";
    }

    if (activeTab === "feature-suggestion") {
      return "Comparte tu idea. Queremos construir features que realmente te ayuden.";
    }

    return "Preguntas rápidas para ayudarte sin fricción.";
  }, [activeTab]);

  const clearFeedbackState = () => {
    submissionMutation.reset();
  };

  const closePanel = () => {
    setIsOpen(false);
    clearFeedbackState();
  };

  const openPanel = () => {
    setIsOpen(true);
  };

  const changeTab = (tab: HelpFeedbackTab) => {
    setActiveTab(tab);
    clearFeedbackState();
  };

  const onSubmit = handleSubmit(async (values) => {
    if (!isFormTab) return;

    try {
      await submissionMutation.mutateAsync({
        type: activeTab,
        payload: {
          name: values.name,
          email: values.email,
          orderId: values.orderId,
          message: values.message,
        },
      });

      reset({
        name: "",
        email: "",
        orderId: "",
        message: "",
      });
    } catch {
      // UI error state is handled by react-query mutation.
    }
  });

  return (
    <>
      <button
        type="button"
        aria-label="Open help and feedback"
        onClick={openPanel}
        className="bg-brand-500 text-brand-50 hover:bg-brand-600 fixed right-5 bottom-5 z-40 flex items-center gap-2 rounded-full px-4 py-3 shadow-lg transition-all duration-200 hover:shadow-xl"
      >
        <MessageCircleQuestion size={18} />
        <span className="text-sm font-semibold">Help</span>
      </button>

      {isOpen && (
        <div className="fixed inset-0 z-50 flex justify-end bg-black/40">
          <div
            role="dialog"
            aria-modal="true"
            aria-label="Help and feedback panel"
            className="flex h-full w-full max-w-xl flex-col bg-white shadow-2xl"
          >
            <div className="bg-brand-500 text-brand-50 flex items-start justify-between px-6 py-5">
              <div>
                <h2 className="text-xl font-bold">{formTitle}</h2>
                <p className="mt-1 text-sm text-white/90">{formDescription}</p>
              </div>
              <button
                type="button"
                aria-label="Close help panel"
                onClick={closePanel}
                className="hover:bg-brand-600 rounded-full p-2 transition"
              >
                <X size={18} />
              </button>
            </div>

            <div className="border-brand-100 flex border-b px-4 pt-4">
              {tabs.map((tab) => {
                const isActive = activeTab === tab.key;

                return (
                  <button
                    key={tab.key}
                    type="button"
                    role="tab"
                    aria-selected={isActive}
                    onClick={() => changeTab(tab.key)}
                    className={`mr-2 rounded-t-xl border px-4 py-2 text-sm font-semibold transition ${
                      isActive
                        ? "border-brand-500 bg-brand-500 text-white"
                        : "border-transparent bg-brand-50 text-brand-500 hover:bg-brand-100"
                    }`}
                  >
                    {tab.label}
                  </button>
                );
              })}
            </div>

            <div className="flex-1 overflow-y-auto px-6 py-5">
              {activeTab === "faq" && (
                <div className="space-y-4" role="tabpanel" aria-label="FAQ tab content">
                  {faqItems.map((faq) => (
                    <article key={faq.question} className="rounded-xl border border-brand-100 p-4">
                      <h3 className="text-brand-600 mb-2 text-sm font-bold uppercase tracking-wide">
                        {faq.question}
                      </h3>
                      <p className="text-brand-500 text-sm leading-relaxed">{faq.answer}</p>
                    </article>
                  ))}
                </div>
              )}

              {isFormTab && (
                <form
                  className="space-y-4"
                  onSubmit={onSubmit}
                  role="tabpanel"
                  aria-label={`${activeTab} tab content`}
                >
                  <div>
                    <label htmlFor="help-name" className="text-brand-600 mb-1 block text-sm font-medium">
                      Nombre
                    </label>
                    <input
                      id="help-name"
                      type="text"
                      className="outline-brand-500 text-brand-500 w-full rounded-xl border border-brand-100 bg-white p-3 outline-2 focus:outline-3"
                      placeholder="Tu nombre"
                      {...register("name", {
                        required: "El nombre es obligatorio.",
                      })}
                    />
                    {errors.name && <p className="mt-1 text-sm text-red-500">{errors.name.message}</p>}
                  </div>

                  <div>
                    <label htmlFor="help-email" className="text-brand-600 mb-1 block text-sm font-medium">
                      Email
                    </label>
                    <input
                      id="help-email"
                      type="email"
                      className="outline-brand-500 text-brand-500 w-full rounded-xl border border-brand-100 bg-white p-3 outline-2 focus:outline-3"
                      placeholder="nombre@email.com"
                      {...register("email", {
                        required: "El email es obligatorio.",
                        pattern: {
                          value: /^[\w-.]+@([\w-]+\.)+[\w-]{2,4}$/,
                          message: "Ingresa un email válido.",
                        },
                      })}
                    />
                    {errors.email && <p className="mt-1 text-sm text-red-500">{errors.email.message}</p>}
                  </div>

                  <div>
                    <label htmlFor="help-order" className="text-brand-600 mb-1 block text-sm font-medium">
                      Número de orden (opcional)
                    </label>
                    <input
                      id="help-order"
                      type="text"
                      className="outline-brand-500 text-brand-500 w-full rounded-xl border border-brand-100 bg-white p-3 outline-2 focus:outline-3"
                      placeholder="ORD-10293"
                      {...register("orderId")}
                    />
                  </div>

                  <div>
                    <label htmlFor="help-message" className="text-brand-600 mb-1 block text-sm font-medium">
                      {activeTab === "bug-report" ? "Describe el bug" : "Describe tu sugerencia"}
                    </label>
                    <textarea
                      id="help-message"
                      rows={5}
                      className="outline-brand-500 text-brand-500 w-full rounded-xl border border-brand-100 bg-white p-3 outline-2 focus:outline-3"
                      placeholder={
                        activeTab === "bug-report"
                          ? "Ejemplo: al intentar pagar, el botón se queda cargando..."
                          : "Ejemplo: me gustaría guardar múltiples direcciones favoritas..."
                      }
                      {...register("message", {
                        required: "Este campo es obligatorio.",
                        minLength: {
                          value: 12,
                          message: "Por favor agrega más detalles (mínimo 12 caracteres).",
                        },
                      })}
                    />
                    {errors.message && (
                      <p className="mt-1 text-sm text-red-500">{errors.message.message}</p>
                    )}
                  </div>

                  {submissionMutation.isSuccess && (
                    <p className="rounded-xl border border-green-200 bg-green-50 px-3 py-2 text-sm text-green-700">
                      ¡Recibimos tu mensaje! Te responderemos pronto.
                    </p>
                  )}

                  {submissionMutation.isError && (
                    <p className="rounded-xl border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">
                      {(submissionMutation.error as Error)?.message ||
                        "No pudimos enviar tu mensaje ahora mismo."}
                    </p>
                  )}

                  <div className="pt-2">
                    <button
                      type="submit"
                      disabled={submissionMutation.isPending}
                      className="bg-secondary-500 text-brand-50 hover:bg-secondary-600 disabled:bg-secondary-300 inline-flex items-center gap-2 rounded-xl px-5 py-3 text-sm font-semibold transition disabled:cursor-not-allowed"
                    >
                      {activeTab === "bug-report" ? <Bug size={16} /> : <Lightbulb size={16} />}
                      {submissionMutation.isPending ? "Enviando..." : "Enviar"}
                    </button>
                  </div>
                </form>
              )}
            </div>
          </div>
        </div>
      )}
    </>
  );
}

export default HelpFeedbackWidget;
