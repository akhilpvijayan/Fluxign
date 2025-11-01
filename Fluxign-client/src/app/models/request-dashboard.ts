import { Time } from "@angular/common";

export interface RequestDashboard {
    requestId: string;
    title: string;
    createdAt: string;
    status: string;
    recipients: Recipient[];
  }

  export interface Recipient {
    name: string;
    email: string;
    phone: string;
    avatarUrl: string;
    signed: boolean;
    rejected: boolean;
    action: string;
    timestamp?: string | null;
    order: number;
    positions?: SignerPosition[];
  }

  export interface SignerPosition {
    page: number;
    x: number;
    y: number;
  }