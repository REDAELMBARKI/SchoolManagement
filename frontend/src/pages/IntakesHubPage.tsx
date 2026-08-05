import { Link } from "react-router-dom";

type HubCard = {
  icon: string;
  label: string;
  description: string;
  href: string;
  accent: string; // tailwind bg colour for the left stripe
};

const cards: HubCard[] = [
  {
    icon: "/lesson.png",
    label: "All Intakes",
    description: "Browse and manage every student intake record.",
    href: "/list/intakes",
    accent: "bg-lamaSky",
  },
  {
    icon: "/create.png",
    label: "New Intake",
    description: "Register a new student intake from scratch.",
    href: "/list/intakes/new",
    accent: "bg-lamaPurple",
  },
];

export default function IntakesHubPage() {
  return (
    <div className="p-6 flex flex-col gap-6">
      {/* Header */}
      <div className="flex flex-col gap-1">
        <h1 className="text-2xl font-bold text-gray-800 tracking-tight">
          Intakes
        </h1>
        <p className="text-sm text-gray-400">
          Manage student intakes and registrations.
        </p>
      </div>

      {/* Divider */}
      <div className="h-px bg-gray-100" />

      {/* Cards grid */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
        {cards.map((card) => (
          <Link
            key={card.label}
            to={card.href}
            className="group relative flex items-stretch bg-white rounded-xl border border-gray-100 shadow-sm hover:shadow-md hover:border-lamaSky transition-all duration-200 overflow-hidden"
          >
            {/* Left accent stripe */}
            <div
              className={`w-1 flex-shrink-0 ${card.accent} opacity-70 group-hover:opacity-100 transition-opacity`}
            />

            {/* Card body */}
            <div className="flex items-center gap-4 px-5 py-5 flex-1">
              {/* Icon container */}
              <div className="flex-shrink-0 w-11 h-11 rounded-lg bg-gray-50 border border-gray-100 flex items-center justify-center group-hover:border-lamaSky transition-colors">
                <img src={card.icon} alt="" width={22} height={22} />
              </div>

              {/* Text */}
              <div className="flex flex-col gap-0.5">
                <span className="font-semibold text-gray-800 text-sm group-hover:text-lamaSky transition-colors">
                  {card.label}
                </span>
                <span className="text-xs text-gray-400 leading-relaxed">
                  {card.description}
                </span>
              </div>

              {/* Arrow */}
              <div className="ml-auto text-gray-300 group-hover:text-lamaSky group-hover:translate-x-0.5 transition-all text-lg leading-none">
                →
              </div>
            </div>
          </Link>
        ))}
      </div>
    </div>
  );
}
