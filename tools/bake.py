import struct
import sys
import zlib

import texture2ddecoder
import UnityPy

ICONS = ("SidePowerHitSlotIcon", "StingerSlotIcon")


def levels(width, height, count):
    out = []
    w, h = width, height
    for _ in range(count):
        out.append((w, h))
        w, h = max(1, w // 2), max(1, h // 2)
    return out


def decode(tex, data):
    if tex["m_TextureFormat"] != 12:
        raise SystemExit("texture %s is format %d, this bake reads DXT5 (12) only" % (tex["m_Name"], tex["m_TextureFormat"]))
    raw = bytearray()
    at = 0
    for w, h in levels(tex["m_Width"], tex["m_Height"], tex["m_MipCount"]):
        bw, bh = max(4, (w + 3) // 4 * 4), max(4, (h + 3) // 4 * 4)
        size = bw * bh
        bgra = texture2ddecoder.decode_bc3(data[at:at + size], bw, bh)
        at += size
        for y in range(h):
            row = bgra[y * bw * 4:(y * bw + w) * 4]
            for x in range(0, len(row), 4):
                raw += bytes((row[x + 2], row[x + 1], row[x], row[x + 3]))
    if at != len(data):
        raise SystemExit("texture %s: read %d of %d bytes, the mip layout is not what this bake expects" % (tex["m_Name"], at, len(data)))
    return bytes(raw)


def main(bundle, out):
    env = UnityPy.load(bundle)
    found = {}
    for obj in env.objects:
        if obj.type.name == "Texture2D":
            tex = obj.read_typetree()
            found[tex["m_Name"]] = (tex, bytes(obj.read().get_image_data()))
    blob = bytearray(b"SRQ1")
    blob += struct.pack("<i", len(ICONS))
    for name in ICONS:
        if name not in found:
            raise SystemExit("the bundle has no texture " + name)
        tex, data = found[name]
        raw = decode(tex, data)
        packed = zlib.compress(raw, 9)[2:-4]
        s = tex["m_TextureSettings"]
        encoded = name.encode("utf-8")
        blob += struct.pack("<i", len(encoded)) + encoded
        blob += struct.pack("<iiiiiiii", tex["m_Width"], tex["m_Height"], tex["m_MipCount"], s["m_FilterMode"], s["m_WrapU"], s["m_Aniso"], len(raw), len(packed))
        blob += packed
        print("%s %dx%d mips %d raw %d packed %d" % (name, tex["m_Width"], tex["m_Height"], tex["m_MipCount"], len(raw), len(packed)))
    with open(out, "wb") as f:
        f.write(blob)
    print("wrote %s %d bytes" % (out, len(blob)))


if __name__ == "__main__":
    if len(sys.argv) != 3:
        raise SystemExit("usage: bake.py <CGNik's !stingerreadded bundle> <out icons.bin>")
    main(sys.argv[1], sys.argv[2])
