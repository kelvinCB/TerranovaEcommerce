import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it, vi, beforeEach, afterEach } from "vitest";
import HelpFeedbackWidget from "@/features/help-feedback/components/HelpFeedbackWidget";

function renderWithProviders() {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false },
      mutations: { retry: false },
    },
  });

  return render(
    <QueryClientProvider client={queryClient}>
      <HelpFeedbackWidget />
    </QueryClientProvider>,
  );
}

describe("HelpFeedbackWidget", () => {
  beforeEach(() => {
    vi.stubGlobal("fetch", vi.fn());
  });

  afterEach(() => {
    vi.unstubAllGlobals();
    vi.clearAllMocks();
  });

  it("renders help FAB and opens/closes panel", () => {
    renderWithProviders();

    const fabButton = screen.getByLabelText("Open help and feedback");
    expect(fabButton).toBeInTheDocument();

    fireEvent.click(fabButton);
    expect(screen.getByRole("dialog", { name: "Help and feedback panel" })).toBeInTheDocument();

    fireEvent.click(screen.getByLabelText("Close help panel"));
    expect(screen.queryByRole("dialog", { name: "Help and feedback panel" })).not.toBeInTheDocument();
  });

  it("switches between tabs and shows FAQ content by default", () => {
    renderWithProviders();
    fireEvent.click(screen.getByLabelText("Open help and feedback"));

    expect(screen.getByText("¿Cómo rastreo mi orden?")).toBeInTheDocument();

    fireEvent.click(screen.getByRole("tab", { name: "Bug Report" }));
    expect(screen.getByLabelText("Describe el bug")).toBeInTheDocument();

    fireEvent.click(screen.getByRole("tab", { name: "Feature Suggestion" }));
    expect(screen.getByLabelText("Describe tu sugerencia")).toBeInTheDocument();
  });

  it("shows form validation messages", async () => {
    renderWithProviders();
    fireEvent.click(screen.getByLabelText("Open help and feedback"));
    fireEvent.click(screen.getByRole("tab", { name: "Bug Report" }));

    fireEvent.click(screen.getByRole("button", { name: "Enviar" }));

    expect(await screen.findByText("El nombre es obligatorio.")).toBeInTheDocument();
    expect(screen.getByText("El email es obligatorio.")).toBeInTheDocument();
    expect(screen.getByText("Este campo es obligatorio.")).toBeInTheDocument();
  });

  it("submits bug report and shows success message", async () => {
    (fetch as unknown as ReturnType<typeof vi.fn>).mockResolvedValue({
      ok: true,
      json: vi.fn().mockResolvedValue({ success: true }),
    });

    renderWithProviders();
    fireEvent.click(screen.getByLabelText("Open help and feedback"));
    fireEvent.click(screen.getByRole("tab", { name: "Bug Report" }));

    fireEvent.change(screen.getByLabelText("Nombre"), {
      target: { value: "Kelvin" },
    });
    fireEvent.change(screen.getByLabelText("Email"), {
      target: { value: "kelvin@mail.com" },
    });
    fireEvent.change(screen.getByLabelText("Describe el bug"), {
      target: { value: "El checkout se congela al confirmar pago." },
    });

    fireEvent.click(screen.getByRole("button", { name: "Enviar" }));

    await waitFor(() => {
      expect(fetch).toHaveBeenCalledWith(
        "/api/help-feedback/bug-report",
        expect.objectContaining({ method: "POST" }),
      );
    });

    expect(
      await screen.findByText("¡Recibimos tu mensaje! Te responderemos pronto."),
    ).toBeInTheDocument();
  });

  it("submits feature suggestion to the right endpoint and resets form", async () => {
    (fetch as unknown as ReturnType<typeof vi.fn>).mockResolvedValue({
      ok: true,
      json: vi.fn().mockResolvedValue({ success: true }),
    });

    renderWithProviders();
    fireEvent.click(screen.getByLabelText("Open help and feedback"));
    fireEvent.click(screen.getByRole("tab", { name: "Feature Suggestion" }));

    fireEvent.change(screen.getByLabelText("Nombre"), {
      target: { value: "Kelvin" },
    });
    fireEvent.change(screen.getByLabelText("Email"), {
      target: { value: "kelvin@mail.com" },
    });
    fireEvent.change(screen.getByLabelText("Número de orden (opcional)"), {
      target: { value: "ORD-1001" },
    });
    fireEvent.change(screen.getByLabelText("Describe tu sugerencia"), {
      target: { value: "Agregar atajos para recompra desde historial." },
    });

    fireEvent.click(screen.getByRole("button", { name: "Enviar" }));

    await waitFor(() => {
      expect(fetch).toHaveBeenCalledWith(
        "/api/help-feedback/feature-suggestion",
        expect.objectContaining({ method: "POST" }),
      );
    });

    await screen.findByText("¡Recibimos tu mensaje! Te responderemos pronto.");

    expect(screen.getByLabelText("Nombre")).toHaveValue("");
    expect(screen.getByLabelText("Email")).toHaveValue("");
    expect(screen.getByLabelText("Número de orden (opcional)")).toHaveValue("");
    expect(screen.getByLabelText("Describe tu sugerencia")).toHaveValue("");
  });

  it("shows loading and error states during submit", async () => {
    (fetch as unknown as ReturnType<typeof vi.fn>).mockImplementation(
      () =>
        new Promise((resolve) => {
          setTimeout(() => {
            resolve({
              ok: false,
              json: vi.fn().mockResolvedValue({}),
            });
          }, 30);
        }),
    );

    renderWithProviders();
    fireEvent.click(screen.getByLabelText("Open help and feedback"));
    fireEvent.click(screen.getByRole("tab", { name: "Feature Suggestion" }));

    fireEvent.change(screen.getByLabelText("Nombre"), {
      target: { value: "Kelvin" },
    });
    fireEvent.change(screen.getByLabelText("Email"), {
      target: { value: "kelvin@mail.com" },
    });
    fireEvent.change(screen.getByLabelText("Describe tu sugerencia"), {
      target: { value: "Agregar listas de compras compartidas para familias." },
    });

    fireEvent.click(screen.getByRole("button", { name: "Enviar" }));

    expect(await screen.findByRole("button", { name: "Enviando..." })).toBeDisabled();

    expect(
      await screen.findByText("No pudimos enviar tu solicitud. Inténtalo de nuevo."),
    ).toBeInTheDocument();
  });
});
