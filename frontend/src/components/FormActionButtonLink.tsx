import { Link } from "react-router-dom";

const sizeByVariant = {
  create: "w-8 h-8",
  update: "w-7 h-7",
  delete: "w-7 h-7",
} as const;

const bgByVariant = {
  create: "bg-lamaYellow",
  update: "bg-lamaSky",
  delete: "bg-lamaPurple",
} as const;

const iconByVariant = {
  create: "/create.png",
  update: "/update.png",
  delete: "/delete.png",
} as const;

type Variant = keyof typeof sizeByVariant;

export default function FormActionButtonLink({
  to,
  variant,
  className = "",
}: {
  to: string;
  variant: Variant;
  className?: string;
}) {
  return (
    <Link
      to={to}
      className={`${sizeByVariant[variant]} flex items-center justify-center rounded-full ${bgByVariant[variant]} ${className} hover:opacity-90`}
    >
      <img src={iconByVariant[variant]} alt="" width={16} height={16} />
    </Link>
  );
}

